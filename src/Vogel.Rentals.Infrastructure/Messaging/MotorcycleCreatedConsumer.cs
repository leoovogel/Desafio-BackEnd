using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Infrastructure.Contexts;

namespace Vogel.Rentals.Infrastructure.Messaging;

public class MotorcycleCreatedConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitmQOptions> options,
    ILogger<MotorcycleCreatedConsumer> logger)
    : BackgroundService
{
    private readonly RabbitmQOptions _options = options.Value;

    private IConnection? _connection;
    private IModel? _channel;

    private void EnsureConnection()
    {
        if (_connection != null && _channel != null)
            return;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        logger.LogInformation("Connecting to RabbitMQ at {Host}...", _options.HostName);

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        logger.LogInformation("RabbitMQ connection established and queue {Queue} declared.", _options.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureConnection();
                break;
            }
            catch (BrokerUnreachableException ex)
            {
                logger.LogWarning(ex, "RabbitMQ ainda não está disponível. Tentando novamente em 5 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao conectar ao RabbitMQ. Tentando novamente em 5 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        if (_channel is null)
        {
            logger.LogError("Não foi possível criar o channel do RabbitMQ. Consumer não será iniciado.");
            return;
        }

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var payload = JsonSerializer.Deserialize<MotorcycleCreatedPayload>(json);
                if (payload is null)
                {
                    logger.LogWarning("Mensagem inválida recebida da fila {Queue}.", _options.QueueName);
                    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                    return;
                }

                if (payload.ano == 2024)
                {
                    using var scope = scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<RentalsDbContext>();

                    var notification = new MotorcycleNotification
                    {
                        Identifier = payload.identificador,
                        Year       = payload.ano,
                        Model      = payload.modelo,
                        Plate      = payload.placa,
                        CreatedAt  = payload.created_at
                    };

                    db.MotorcycleNotifications.Add(notification);
                    await db.SaveChangesAsync();

                    logger.LogInformation(
                        "Notificação de moto 2024 salva no banco. Identifier={Identifier}, Plate={Plate}",
                        notification.Identifier,
                        notification.Plate);
                }

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem da fila {Queue}. Dando NACK e requeue.",
                    _options.QueueName);
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);

        logger.LogInformation("MotorcycleCreatedConsumer iniciado. Aguardando mensagens...");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }

    private sealed class MotorcycleCreatedPayload
    {
        public string identificador { get; set; } = default!;
        public int ano { get; set; }
        public string modelo { get; set; } = default!;
        public string placa { get; set; } = default!;
        public DateTime created_at { get; set; }
    }
}
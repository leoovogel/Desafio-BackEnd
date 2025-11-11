using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Infrastructure.Messaging;

public class RabbitMqMotorcycleEventPublisher : IMotorcycleEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitmQOptions _options;

    public RabbitMqMotorcycleEventPublisher(IOptions<RabbitmQOptions> options)
    {
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public Task PublishMotorcycleCreatedAsync(Motorcycle motorcycle)
    {
        var payload = new
        {
            identificador = motorcycle.Identifier,
            ano = motorcycle.Year,
            modelo = motorcycle.Model,
            placa = motorcycle.Plate,
            created_at = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: _options.QueueName,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
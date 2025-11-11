namespace Vogel.Rentals.Infrastructure.Messaging;

public class RabbitmQOptions
{
    public string HostName { get; set; } = "rabbitmq";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "motorcycle_created";
}
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface IMotorcycleEventPublisher
{
    Task PublishMotorcycleCreatedAsync(Motorcycle motorcycle);
}
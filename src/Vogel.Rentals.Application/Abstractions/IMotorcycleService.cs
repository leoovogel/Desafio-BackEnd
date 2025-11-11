using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface IMotorcycleService
{
    Task<Motorcycle> CreateAsync(CreateMotorcycleRequest req);
    Task DeleteAsync(string id);
}
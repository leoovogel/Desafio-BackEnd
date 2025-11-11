using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface IMotorcycleRepository
{
    Task<Motorcycle> AddAsync(Motorcycle motorcycle);
    Task<Motorcycle> SearchByIdAsync(string id);
    Task<IEnumerable<Motorcycle>> SearchByPlateAsync(string? plate);
    Task UpdatePlateAsync(string id, string newPlate);
    Task DeleteAsync(string id);
}
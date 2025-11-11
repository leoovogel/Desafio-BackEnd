using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Application.Abstractions;

public interface IMotorcycleRepository
{
    Task<Motorcycle> AddAsync(Motorcycle motorcycle);
    Task<IEnumerable<Motorcycle>> SearchAsync(string? id);
    Task<IEnumerable<Motorcycle>> SearchByPlateAsync(string? plate);
    Task<bool> UpdatePlateAsync(string id, string newPlate);
    Task<bool> DeleteAsync(string id);
    Task<Motorcycle?> GetByIdAsync(string id);
}
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Application.Abstractions;

public interface IMotorcycleRepository
{
    Task<Motorcycle> AddAsync(Motorcycle motorcycle);
    Task<bool> PlateExistsAsync(string plate);
    Task<IEnumerable<Motorcycle>> SearchAsync(string? plate);
    Task<bool> UpdatePlateAsync(string currentPlate, string newPlate);
}
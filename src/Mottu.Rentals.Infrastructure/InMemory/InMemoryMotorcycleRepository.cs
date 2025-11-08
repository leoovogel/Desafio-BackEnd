using System.Collections.Concurrent;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Infrastructure.InMemory;

public class InMemoryMotorcycleRepository : IMotorcycleRepository
{
    private readonly ConcurrentDictionary<Guid, Motorcycle> _store = new();
    private readonly ConcurrentDictionary<string, Guid> _plates = new(StringComparer.OrdinalIgnoreCase);

    private static string NormalizePlate(string plate) => plate.Trim().ToUpperInvariant();

    public Task<Motorcycle> AddAsync(Motorcycle motorcycle)
    {
        ArgumentNullException.ThrowIfNull(motorcycle);

        if (string.IsNullOrWhiteSpace(motorcycle.Identifier) ||
            string.IsNullOrWhiteSpace(motorcycle.Model) ||
            string.IsNullOrWhiteSpace(motorcycle.Plate) ||
            motorcycle.Year <= 0)
            throw new InvalidOperationException("invalid motorcycle data");

        var plate = NormalizePlate(motorcycle.Plate);
        var id = motorcycle.Id == Guid.Empty ? Guid.NewGuid() : motorcycle.Id;

        motorcycle = new Motorcycle
        {
            Id = id,
            Identifier = motorcycle.Identifier.Trim(),
            Year = motorcycle.Year,
            Model = motorcycle.Model.Trim(),
            Plate = plate
        };

        if (!_plates.TryAdd(plate, id))
            throw new InvalidOperationException("plate already exists");

        _store[id] = motorcycle;
        return Task.FromResult(motorcycle);
    }

    public Task<bool> PlateExistsAsync(string plate)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(plate) && _plates.ContainsKey(NormalizePlate(plate)));
    }

    public Task<IEnumerable<Motorcycle>> SearchAsync(string? plate)
    {
        IEnumerable<Motorcycle> motorcycles = _store.Values;
        
        if (string.IsNullOrWhiteSpace(plate))
            return Task.FromResult(motorcycles);
        
        var normalizedPlate = NormalizePlate(plate);
        motorcycles = motorcycles.Where(m => string.Equals(m.Plate, normalizedPlate, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(motorcycles);
    }
}
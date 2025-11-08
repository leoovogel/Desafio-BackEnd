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

    public Task<bool> UpdatePlateAsync(string currentPlate, string newPlate)
    {
        if (string.IsNullOrWhiteSpace(currentPlate) || string.IsNullOrWhiteSpace(newPlate))
            return Task.FromResult(false);
    
        var normalizedCurrent = NormalizePlate(currentPlate);
        var normalizedNew = NormalizePlate(newPlate);
    
        if (string.Equals(normalizedCurrent, normalizedNew, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(true);
    
        if (!_plates.TryGetValue(normalizedCurrent, out var id) || !_store.TryGetValue(id, out var current) || !_plates.TryAdd(normalizedNew, id))
            return Task.FromResult(false);
        
        _plates.TryRemove(normalizedCurrent, out _);
        var updated = new Motorcycle
        {
            Id = current.Id,
            Identifier = current.Identifier,
            Year = current.Year,
            Model = current.Model,
            Plate = normalizedNew
        };
    
        _store[id] = updated;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByPlateAsync(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return Task.FromResult(false);

        var normalized = NormalizePlate(plate);

        if (!_plates.TryGetValue(normalized, out var id))
            return Task.FromResult(false);

        _plates.TryRemove(normalized, out _);
        _store.TryRemove(id, out _);

        return Task.FromResult(true);
    }
}
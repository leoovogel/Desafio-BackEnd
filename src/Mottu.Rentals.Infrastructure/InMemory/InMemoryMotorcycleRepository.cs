using System.Collections.Concurrent;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Infrastructure.InMemory;

public class InMemoryMotorcycleRepository : IMotorcycleRepository
{
    private readonly ConcurrentDictionary<string, Motorcycle> _store = new();
    private readonly ConcurrentDictionary<string, string> _plates = new();

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

        motorcycle = new Motorcycle
        {
            Identifier = motorcycle.Identifier.Trim(),
            Year = motorcycle.Year,
            Model = motorcycle.Model.Trim(),
            Plate = plate.Trim()
        };

        if (!_plates.TryAdd(plate, motorcycle.Identifier))
            throw new InvalidOperationException("plate already exists");

        _store[motorcycle.Identifier] = motorcycle;
        return Task.FromResult(motorcycle);
    }

    public Task<bool> PlateExistsAsync(string plate)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(plate) && _plates.ContainsKey(NormalizePlate(plate)));
    }

    public Task<IEnumerable<Motorcycle>> SearchAsync(string? id)
    {
        IEnumerable<Motorcycle> motorcycles = _store.Values;
        
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(motorcycles);
        
        motorcycles = motorcycles.Where(m => string.Equals(m.Identifier, id));
        return Task.FromResult(motorcycles);
    }
    
    public Task<IEnumerable<Motorcycle>> SearchByPlateAsync(string? plate)
    {
        IEnumerable<Motorcycle> motorcycles = _store.Values;
        
        if (string.IsNullOrWhiteSpace(plate))
            return Task.FromResult(motorcycles);
        
        var normalizedPlate = NormalizePlate(plate);
        motorcycles = motorcycles.Where(m => string.Equals(m.Plate, normalizedPlate));
        return Task.FromResult(motorcycles);
    }

    public Task<bool> UpdatePlateAsync(string id, string newPlate)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newPlate))
            return Task.FromResult(false);
    
        var normalizedNew = NormalizePlate(newPlate);
    
        if (!_store.TryGetValue(id, out var motorcycle) || !_plates.TryAdd(normalizedNew, id))
            return Task.FromResult(false);

        _plates.TryRemove(motorcycle.Plate, out _);

        var updated = new Motorcycle
        {
            Identifier = motorcycle.Identifier,
            Year = motorcycle.Year,
            Model = motorcycle.Model,
            Plate = normalizedNew
        };
    
        _store[id] = updated;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(false);

        if (!_store.TryRemove(id, out var motorcycle))
            return Task.FromResult(false);

        _plates.TryRemove(motorcycle.Plate, out _);

        return Task.FromResult(true);
    }
    
    public Task<Motorcycle?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult<Motorcycle?>(null);

        var moto = _store.Values.FirstOrDefault(m => string.Equals(m.Identifier, id));

        return Task.FromResult(moto);
    }
}
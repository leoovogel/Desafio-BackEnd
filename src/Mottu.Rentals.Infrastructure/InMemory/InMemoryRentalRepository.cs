using System.Collections.Concurrent;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Infrastructure.InMemory;

public class InMemoryRentalRepository : IRentalRepository
{
    private readonly ConcurrentDictionary<Guid, Rental> _store = new();

    public Task<Rental> AddAsync(Rental rental)
    {
        _store[rental.Identifier] = rental;
        return Task.FromResult(rental);
    }

    public Task<Rental?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var rental);
        return Task.FromResult(rental);
    }
}
// using System.Collections.Concurrent;
// using Vogel.Rentals.Application.Abstractions;
// using Vogel.Rentals.Domain.Entities;
//
// namespace Vogel.Rentals.Infrastructure.InMemory;
//
// public class InMemoryRentalRepository : IRentalRepository
// {
//     private readonly ConcurrentDictionary<Guid, Rental> _store = new();
//
//     public Task<Rental> AddAsync(Rental rental)
//     {
//         _store[rental.Identifier] = rental;
//         return Task.FromResult(rental);
//     }
//
//     public Task<Rental?> GetByIdAsync(Guid id)
//     {
//         _store.TryGetValue(id, out var rental);
//         return Task.FromResult(rental);
//     }
//
//     public Task<Rental> UpdateAsync(Rental rental)
//     {
//         if (rental.Identifier == Guid.Empty)
//             throw new InvalidOperationException("Rental must have an Id to be updated.");
//
//         _store[rental.Identifier] = rental;
//         return Task.FromResult(rental);
//     }
// }
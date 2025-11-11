using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Application.Abstractions;

public interface IRentalRepository
{
    Task<Rental> AddAsync(Rental rental);
    Task<Rental?> GetByIdAsync(Guid id);
    Task<Rental> UpdateAsync(Rental rental);
}
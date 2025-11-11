using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface IRentalService
{
    Task<Rental> CreateAsync(CreateRentalRequest req);
    Task<RentalResponse> GetByIdAsync(Guid id);
    Task<RentalTotalValueResponse> RentalReturnAsync(Guid id, DateTime returnDate);
}
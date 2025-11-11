using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;
using Vogel.Rentals.Infrastructure.Contexts;

namespace Vogel.Rentals.Infrastructure.Repositories;

public class RentalRepository(RentalsDbContext db) : IRentalRepository
{
    public async Task<Rental> AddAsync(Rental rental)
    {
        if (rental is null)
            throw new BusinessRuleException();

        if (rental.StartDate == default ||
            rental.EndDate == default ||
            rental.ExpectedEndDate == default ||
            string.IsNullOrWhiteSpace(rental.CourierId) ||
            string.IsNullOrWhiteSpace(rental.MotorcycleId))
        {
            throw new BusinessRuleException();
        }

        if (rental.Identifier == Guid.Empty)
            rental.Identifier = Guid.NewGuid();
        
        db.Rentals.Add(rental);
        await db.SaveChangesAsync();
        return rental;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await db.Rentals
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Identifier == id);
    }

    public async Task<Rental> UpdateAsync(Rental rental)
    {
        if (rental is null || rental.Identifier == Guid.Empty)
            throw new BusinessRuleException();
        
        var exists = await db.Rentals
            .AnyAsync(r => r.Identifier == rental.Identifier);

        if (!exists)
            throw new BusinessRuleException();

        db.Rentals.Update(rental);
        await db.SaveChangesAsync();
        return rental;
    }
}
using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;
using Vogel.Rentals.Infrastructure.Contexts;

namespace Vogel.Rentals.Infrastructure.Repositories;

public class MotorcycleRepository(RentalsDbContext db) : IMotorcycleRepository
{
    private static string NormalizePlate(string plate) => plate.Trim().ToUpperInvariant();

    public async Task<Motorcycle> AddAsync(Motorcycle motorcycle)
    {
        if (string.IsNullOrWhiteSpace(motorcycle.Identifier) ||
            string.IsNullOrWhiteSpace(motorcycle.Model) ||
            string.IsNullOrWhiteSpace(motorcycle.Plate) ||
            motorcycle.Year <= 0)
        {
            throw new BusinessRuleException();
        }

        var plate = NormalizePlate(motorcycle.Plate);

        var plateInUse = await db.Motorcycles.AnyAsync(m => m.Plate == plate);
        if (plateInUse)
            throw new BusinessRuleException();

        motorcycle = new Motorcycle
        {
            Identifier = motorcycle.Identifier.Trim(),
            Year = motorcycle.Year,
            Model = motorcycle.Model.Trim(),
            Plate = plate
        };

        db.Motorcycles.Add(motorcycle);
        await db.SaveChangesAsync();

        return motorcycle;
    }

    public async Task<Motorcycle?> GetByIdAsync(string id)
    {
        return await db.Motorcycles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Identifier == id);
    }

    public async Task<IEnumerable<Motorcycle>> GetByPlateAsync(string? plate)
    {
        var query = db.Motorcycles.AsNoTracking();

        if (string.IsNullOrWhiteSpace(plate))
            return await query.ToListAsync();

        var normalized = NormalizePlate(plate);
        query = query.Where(m => m.Plate == normalized);

        return await query.ToListAsync();
    }

    public async Task UpdatePlateAsync(string id, string newPlate)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newPlate))
            throw new BusinessRuleException();

        var normalizedNew = NormalizePlate(newPlate);

        var motorcycle = await db.Motorcycles.FirstOrDefaultAsync(m => m.Identifier == id);
        if (motorcycle is null)
            throw new BusinessRuleException();

        if (string.Equals(motorcycle.Plate, normalizedNew))
            return;

        var plateInUse = await db.Motorcycles.AnyAsync(m => m.Plate == normalizedNew);
        if (plateInUse)
            throw new BusinessRuleException();

        motorcycle.Plate = normalizedNew;
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new BusinessRuleException();

        var motorcycle = await db.Motorcycles.FirstOrDefaultAsync(m => m.Identifier == id);
        if (motorcycle is null)
            throw new BusinessRuleException();

        db.Motorcycles.Remove(motorcycle);
        await db.SaveChangesAsync();
    }
}
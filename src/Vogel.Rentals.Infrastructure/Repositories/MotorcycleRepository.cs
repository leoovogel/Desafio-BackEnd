using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Infrastructure.Contexts;

namespace Vogel.Rentals.Infrastructure.Repositories;

public class MotorcycleRepository(RentalsDbContext db) : IMotorcycleRepository
{
    private static string NormalizePlate(string plate) => plate.Trim().ToUpperInvariant();

    public async Task<Motorcycle> AddAsync(Motorcycle motorcycle)
    {
        ArgumentNullException.ThrowIfNull(motorcycle);

        if (string.IsNullOrWhiteSpace(motorcycle.Identifier) ||
            string.IsNullOrWhiteSpace(motorcycle.Model) ||
            string.IsNullOrWhiteSpace(motorcycle.Plate) ||
            motorcycle.Year <= 0)
        {
            throw new InvalidOperationException("invalid motorcycle data");
        }

        var plate = NormalizePlate(motorcycle.Plate);

        var plateInUse = await db.Motorcycles.AnyAsync(m => m.Plate == plate);
        if (plateInUse)
            throw new InvalidOperationException("plate already exists");

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

    public async Task<IEnumerable<Motorcycle>> SearchAsync(string? id)
    {
        var query = db.Motorcycles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(id))
            query = query.Where(m => m.Identifier == id);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Motorcycle>> SearchByPlateAsync(string? plate)
    {
        var query = db.Motorcycles.AsNoTracking();

        if (string.IsNullOrWhiteSpace(plate))
            return await query.ToListAsync();

        var normalized = NormalizePlate(plate);
        query = query.Where(m => m.Plate == normalized);

        return await query.ToListAsync();
    }

    public async Task<bool> UpdatePlateAsync(string id, string newPlate)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newPlate))
            return false;

        var normalizedNew = NormalizePlate(newPlate);

        var motorcycle = await db.Motorcycles.FirstOrDefaultAsync(m => m.Identifier == id);
        if (motorcycle is null)
            return false;

        if (string.Equals(motorcycle.Plate, normalizedNew))
            return true;

        var plateInUse = await db.Motorcycles.AnyAsync(m => m.Plate == normalizedNew);
        if (plateInUse)
            return false;

        motorcycle.Plate = normalizedNew;
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var motorcycle = await db.Motorcycles.FirstOrDefaultAsync(m => m.Identifier == id);
        if (motorcycle is null)
            return false;

        db.Motorcycles.Remove(motorcycle);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<Motorcycle?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await db.Motorcycles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Identifier == id);
    }
}
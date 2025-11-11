using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;
using Vogel.Rentals.Infrastructure.Contexts;

namespace Vogel.Rentals.Infrastructure.Repositories;

public class CourierRepository(RentalsDbContext db) : ICourierRepository
{
    private static string DigitsOnly(string s) => new(s.Where(char.IsDigit).ToArray());

    public async Task<bool> CnpjExistsAsync(string cnpj)
    {
        var key = DigitsOnly(cnpj);
        return await db.Couriers.AnyAsync(c => c.Cnpj == key);
    }

    public async Task<bool> CnhNumberExistsAsync(string cnhNumber)
    {
        var key = DigitsOnly(cnhNumber);
        return await db.Couriers.AnyAsync(c => c.CnhNumber == key);
    }

    public async Task<Courier?> GetByIdentifierAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await db.Couriers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Identifier == id);
    }

    public async Task<Courier> AddAsync(Courier courier)
    {
        if (courier is null)
            throw new BusinessRuleException();

        var cnpjKey = DigitsOnly(courier.Cnpj);
        var cnhKey = DigitsOnly(courier.CnhNumber);

        var normalized = new Courier
        {
            Identifier = courier.Identifier.Trim(),
            Name = courier.Name.Trim(),
            Cnpj = cnpjKey,
            BirthDate = courier.BirthDate,
            CnhNumber = cnhKey,
            CnhType = courier.CnhType,
            CnhImage = courier.CnhImage
        };

        var exists = await db.Couriers.AnyAsync(c => c.Cnpj == cnpjKey || c.CnhNumber == cnhKey);
        if (exists)
            throw new BusinessRuleException();

        db.Couriers.Add(normalized);
        
        await db.SaveChangesAsync();

        return normalized;
    }

    public async Task UpdateCnhImageAsync(string identifier, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(imageUrl))
            throw new BusinessRuleException();

        var courier = await db.Couriers.FirstOrDefaultAsync(c => c.Identifier == identifier);
        if (courier is null)
            throw new BusinessRuleException();

        courier.CnhImage = imageUrl;

        await db.SaveChangesAsync();
    }
}
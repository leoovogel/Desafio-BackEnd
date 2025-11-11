using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Services;

public class MotorcycleService(
    IMotorcycleRepository motorcycleRepository,
    IRentalRepository rentalRepository)
    : IMotorcycleService
{
    public async Task<Motorcycle> CreateAsync(CreateMotorcycleRequest req)
    {
        var motorcycle = new Motorcycle
        {
            Identifier = req.Identificador,
            Year = req.Ano,
            Model = req.Modelo,
            Plate = req.Placa
        };

        return await motorcycleRepository.AddAsync(motorcycle);
    }

    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new BusinessRuleException();

        var motorcycle = await motorcycleRepository.GetByIdAsync(id);
        if (motorcycle is null)
            throw new NotFoundException("Moto não encontrada");

        var hasRentals = await rentalRepository.HasRentalsForMotorcycleAsync(id);
        if (hasRentals)
            throw new BusinessRuleException();

        await motorcycleRepository.DeleteAsync(id);
    }
}
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Pricing;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Services;

public class RentalService(
    IMotorcycleRepository motorcycleRepository,
    ICourierRepository courierRepository,
    IRentalRepository rentalRepository)
    : IRentalService
{
    public async Task<Rental> CreateAsync(CreateRentalRequest req)
    {
        if (req.DataInicio > req.DataPrevisaoTermino)
            throw new BusinessRuleException();

        if (!RentalPlanCatalog.TryGet(req.Plano, out var plan, out var dailyRate))
            throw new BusinessRuleException();

        var courier = await courierRepository.GetByIdentifierAsync(req.EntregadorId);
        if (courier is null || courier.CnhType != CnhType.A && courier.CnhType != CnhType.AB)
            throw new BusinessRuleException();
        
        var motorcycle = await motorcycleRepository.GetByIdAsync(req.MotoId);
        if (motorcycle is null)
            throw new BusinessRuleException();
        
        var rental = new Rental
        {
            CourierId = req.EntregadorId,
            MotorcycleId = req.MotoId,
            StartDate = req.DataInicio,
            ExpectedEndDate = req.DataPrevisaoTermino,
            EndDate = req.DataTermino,
            Plan = plan,
            DailyRate = dailyRate
        };

        return await rentalRepository.AddAsync(rental);
    }

    public async Task<RentalResponse> GetByIdAsync(Guid id)
    {
        var rental = await rentalRepository.GetByIdAsync(id);
        if (rental is null)
            throw new NotFoundException("Locação não encontrada");
    
        return new RentalResponse(
            rental.Identifier,
            rental.DailyRate,
            rental.CourierId,
            rental.MotorcycleId,
            rental.StartDate,
            rental.EndDate,
            rental.ExpectedEndDate,
            rental.ReturnDate ?? DateTime.MinValue
        );
    }

    public async Task<RentalTotalValueResponse> RentalReturnAsync(Guid id, DateTime returnDate)
    {
        var rental = await rentalRepository.GetByIdAsync(id);
        if (rental is null)
            throw new BusinessRuleException();

        if (returnDate < rental.StartDate)
            throw new BusinessRuleException();

        rental.ReturnDate = returnDate;

        var total = RentalPlanCatalog.CalculateTotal(rental, returnDate);

        await rentalRepository.UpdateAsync(rental);

        return new RentalTotalValueResponse(total, "Data de devolução informada com sucesso");
    }
}
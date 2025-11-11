using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Pricing;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("locacao")]
public class RentalsController(ICourierRepository courierRepository, IMotorcycleRepository motorcycleRepository, IRentalRepository rentalRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRentalRequest? req)
    {
        if (req is null ||
            string.IsNullOrWhiteSpace(req.EntregadorId) ||
            string.IsNullOrWhiteSpace(req.MotoId) ||
            req.DataInicio == default ||
            req.DataTermino == default ||
            req.DataPrevisaoTermino == default)
        {
            return BadRequest(new { mensagem = "Dados inválidos" });
        }
        
        if (req.DataInicio > req.DataPrevisaoTermino)
            return BadRequest(new { mensagem = "Dados inválidos" });

        if (!RentalPlanCatalog.TryGet(req.Plano, out var plan, out var dailyRate))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var courier = await courierRepository.GetByIdentifierAsync(req.EntregadorId);
        if (courier is null || courier.CnhType != CnhType.A && courier.CnhType != CnhType.AB)
            return BadRequest(new { mensagem = "Dados inválidos" });
        
        var motorcycle = await motorcycleRepository.GetByIdAsync(req.MotoId);
        if (motorcycle is null)
            return BadRequest(new { mensagem = "Dados inválidos" });

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

        var saved = await rentalRepository.AddAsync(rental);

        return Created($"/locacoes/{saved.Identifier}", null);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!Guid.TryParse(id, out var idGuid) || idGuid == Guid.Empty)
            return BadRequest(new { mensagem = "Dados inválidos" });

        var rental = await rentalRepository.GetByIdAsync(idGuid);
        if (rental is null)
            return NotFound(new { mensagem = "Locação não encontrada" });
    
        var res = new RentalResponse(
            rental.Identifier,
            rental.DailyRate,
            rental.CourierId,
            rental.MotorcycleId,
            rental.StartDate,
            rental.EndDate,
            rental.ExpectedEndDate,
            rental.ReturnDate ?? DateTime.MinValue
        );

        return Ok(res);
    }

    [HttpPut("{id:guid}/devolucao")]
    public async Task<IActionResult> CalculateTotal(Guid id, [FromBody] CalculateRentalTotalRequest? req)
    {
        if (id == Guid.Empty || req is null || req.DataDevolucao == default)
            return BadRequest(new { mensagem = "Dados inválidos" });

        var rental = await rentalRepository.GetByIdAsync(id);
        if (rental is null)
            return BadRequest(new { mensagem = "Dados inválidos" });

        if (req.DataDevolucao < rental.StartDate)
            return BadRequest(new { mensagem = "Dados inválidos" });
        
        rental.ReturnDate = req.DataDevolucao;

        var total = RentalPlanCatalog.CalculateTotal(rental, req.DataDevolucao);

        await rentalRepository.UpdateAsync(rental);

        var res = new RentalTotalValueResponse(total, "Data de devolução informada com sucesso");
        return Ok(res);
    }
}
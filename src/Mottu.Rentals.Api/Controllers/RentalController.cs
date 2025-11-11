using Microsoft.AspNetCore.Mvc;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Application.Contracts;
using Mottu.Rentals.Application.Pricing;
using Mottu.Rentals.Domain.Entities;
using Mottu.Rentals.Domain.Enums;

namespace Mottu.Rentals.Api.Controllers;

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

        var courier = await courierRepository.GetByIdentificadorAsync(req.EntregadorId);
        if (courier is null)
            // return NotFound(new { mensagem = "Entregador não encontrado" });
            return BadRequest(new { mensagem = "Dados inválidos" });

        if (courier.TipoCnh != CnhType.A && courier.TipoCnh != CnhType.AB)
            // return BadRequest(new { mensagem = "Entregador não habilitado na categoria A" });
            return BadRequest(new { mensagem = "Dados inválidos" });

        var moto = await motorcycleRepository.GetByIdAsync(req.MotoId);
        if (moto is null)
            // return NotFound(new { mensagem = "Moto não encontrada" });
            return BadRequest(new { mensagem = "Dados inválidos" });

        var start = DateOnly.FromDateTime(req.DataInicio);
        var expectedEnd = DateOnly.FromDateTime(req.DataPrevisaoTermino);
        var end = DateOnly.FromDateTime(req.DataTermino);

        if (start > expectedEnd || expectedEnd > end)
            // return BadRequest(new { mensagem = "Período de datas inválido." });
            return BadRequest(new { mensagem = "Dados inválidos" });

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var requiredStart = today.AddDays(1);

        if (start != requiredStart)
            // return BadRequest(new { mensagem = "Data de início deve ser o primeiro dia após a criação da locação." });
            return BadRequest(new { mensagem = "Dados inválidos" });

        if (!RentalPlanCatalog.TryGet(req.Plano, out var plan, out var dailyRate))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var rental = new Rental
        {
            CourierId = req.EntregadorId,
            MotorcycleId = req.MotoId,
            StartDate = start,
            ExpectedEndDate = expectedEnd,
            EndDate = end,
            Plan = plan,
            DailyRate = dailyRate
        };

        var saved = await rentalRepository.AddAsync(rental);

        var res = new RentalResponse(
            saved.Identifier,
            saved.CourierId,
            saved.MotorcycleId,
            saved.StartDate.ToDateTime(TimeOnly.MinValue),
            saved.EndDate.ToDateTime(TimeOnly.MaxValue),
            saved.ExpectedEndDate.ToDateTime(TimeOnly.MaxValue),
            (int)saved.Plan,
            saved.DailyRate
        );

        return Created($"/locacoes/{saved.Identifier}", res);
    }
}
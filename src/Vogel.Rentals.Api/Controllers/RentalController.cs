using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("locacao")]
public class RentalsController(
    IRentalService rentalService,
    IRentalValidator rentalValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRentalRequest? req)
    {
        rentalValidator.ValidateCreate(req);

        var rental = await rentalService.CreateAsync(req);
        return Created($"/locacoes/{rental.Identifier}", null);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var idGuid = rentalValidator.ValidateAndParseGetById(id);
        
        var rental = await rentalService.GetByIdAsync(idGuid);
        return Ok(rental);
    }

    [HttpPut("{id:guid}/devolucao")]
    public async Task<IActionResult> RentalReturn(Guid id, [FromBody] CalculateRentalTotalRequest? req)
    {
        rentalValidator.ValidadeRentalReturn(id, req);

        var res = await rentalService.RentalReturnAsync(id, req.DataDevolucao);
        return Ok(res);
    }
}
using Microsoft.AspNetCore.Mvc;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Application.Contracts;
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Api.Controllers;

[ApiController]
[Route("motorcycles")]
public class MotorcyclesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleRequest req, [FromServices] IMotorcycleRepository repository)
    {
        if (string.IsNullOrWhiteSpace(req.Identifier) ||
            string.IsNullOrWhiteSpace(req.Model) ||
            string.IsNullOrWhiteSpace(req.Plate) ||
            req.Year <= 0)
            return BadRequest(new { error = "Invalid data" });

        if (await repository.PlateExistsAsync(req.Plate))
            return Conflict(new { error = "Plate already exists" });

        var motorcycle = new Motorcycle
        {
            Identifier = req.Identifier.Trim(),
            Year = req.Year,
            Model = req.Model.Trim(),
            Plate = req.Plate.Trim().ToUpperInvariant()
        };

        motorcycle = await repository.AddAsync(motorcycle);
        var res = new MotorcycleResponse(motorcycle.Id, motorcycle.Identifier, motorcycle.Year, motorcycle.Model, motorcycle.Plate);
        return Created($"/motorcycles/{motorcycle.Id}", res);
    }
}
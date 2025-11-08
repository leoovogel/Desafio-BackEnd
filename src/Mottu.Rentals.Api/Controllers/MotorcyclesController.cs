using Microsoft.AspNetCore.Mvc;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Application.Contracts;
using Mottu.Rentals.Domain.Entities;

namespace Mottu.Rentals.Api.Controllers;

[ApiController]
[Route("motorcycles")]
public class MotorcyclesController(IMotorcycleRepository motorcycleRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Identifier) ||
            string.IsNullOrWhiteSpace(req.Model) ||
            string.IsNullOrWhiteSpace(req.Plate) ||
            req.Year <= 0)
            return BadRequest(new { error = "Invalid data" });

        if (await motorcycleRepository.PlateExistsAsync(req.Plate))
            return Conflict(new { error = "Plate already exists" });

        var motorcycle = new Motorcycle
        {
            Identifier = req.Identifier.Trim(),
            Year = req.Year,
            Model = req.Model.Trim(),
            Plate = req.Plate.Trim().ToUpperInvariant()
        };

        motorcycle = await motorcycleRepository.AddAsync(motorcycle);
        var res = new MotorcycleResponse(motorcycle.Id, motorcycle.Identifier, motorcycle.Year, motorcycle.Model, motorcycle.Plate);
        return Created($"/motorcycles/{motorcycle.Id}", res);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? plate)
    {
        var motorcycles = await motorcycleRepository.SearchAsync(plate);
        var res = motorcycles.Select(m => new MotorcycleResponse(m.Id, m.Identifier, m.Year, m.Model, m.Plate));
        return Ok(res);
    }

    [HttpPatch("plate")]
    public async Task<IActionResult> UpdatePlate([FromBody] UpdatePlateByPlateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.CurrentPlate) || string.IsNullOrWhiteSpace(req.NewPlate))
            return BadRequest(new { error = "Invalid data" });
        
        if (!await motorcycleRepository.PlateExistsAsync(req.CurrentPlate))
            return NotFound(new { error = "Motorcycle with current plate not found" });

        if (string.Equals(req.CurrentPlate.Trim(), req.NewPlate.Trim(), StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Current plate and new plate cannot be the same" });
        
        if (await motorcycleRepository.PlateExistsAsync(req.NewPlate))
            return Conflict(new { error = "New plate already exists" });
        
        var updated = await motorcycleRepository.UpdatePlateAsync(req.CurrentPlate, req.NewPlate);
        
        if (!updated)
            return NotFound(new { error = "Default error" });

        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("motos")]
public class MotorcyclesController(
    IMotorcycleRepository motorcycleRepository,
    IMotorcycleService motorcycleService,
    IMotorcycleValidator motorcycleValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleRequest req)
    {
        motorcycleValidator.ValidateCreate(req);

        var motorcycle = await motorcycleService.CreateAsync(req);
        return Created($"/motos/{motorcycle.Identifier}", motorcycle);
    }
    
    [HttpGet]
    public async Task<IActionResult> SearchByPlate([FromQuery] string? placa)
    {
        var motorcycles = await motorcycleRepository.GetByPlateAsync(placa);
        
        return Ok(motorcycles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> SearchById(string id)
    {
        motorcycleValidator.ValidateSearchById(id);
        
        var motorcycles = await motorcycleRepository.GetByIdAsync(id);
        if (motorcycles is null)
            return NotFound(new { mensagem = "Moto nao encontrada" });
        
        return Ok(motorcycles);
    }

    [HttpPut("{id}/placa")]
    public async Task<IActionResult> UpdatePlate([FromBody] UpdatePlateByPlateRequest req, string id)
    {
        motorcycleValidator.ValidateUpdatePlate(id, req);
        
        await motorcycleRepository.UpdatePlateAsync(id, req.Placa);
        return Ok(new { mensagem = "Placa atualizada com sucesso" });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByPlate(string id)
    {
        await motorcycleService.DeleteAsync(id);
        return Ok();
    }
}
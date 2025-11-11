using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("motos")]
public class MotorcyclesController(IMotorcycleRepository motorcycleRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Identificador) ||
            string.IsNullOrWhiteSpace(req.Modelo) ||
            string.IsNullOrWhiteSpace(req.Placa) ||
            req.Ano <= 0)
            return BadRequest(new { mensagem = "Dados inválidos" });

        var motorcycle = new Motorcycle
        {
            Identifier = req.Identificador,
            Year = req.Ano,
            Model = req.Modelo,
            Plate = req.Placa
        };

        var res = await motorcycleRepository.AddAsync(motorcycle);
        return Created($"/motos/{motorcycle.Identifier}", res);
    }
    
    [HttpGet]
    public async Task<IActionResult> SearchByPlate([FromQuery] string? placa)
    {
        var motorcycles = await motorcycleRepository.SearchByPlateAsync(placa);
        
        return Ok(motorcycles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Search(string? id)
    {
        // TODO: Implement request mal formada 400
        if (id != null && string.IsNullOrWhiteSpace(id))
            return BadRequest(new { mensagem = "Request mal formada" });
        
        var motorcycles = await motorcycleRepository.SearchAsync(id);
        
        // TODO: Implement not found 404
        if (id != null && !motorcycles.Any())
            return NotFound(new { mensagem = "Moto não encontrada" });
        
        return Ok(motorcycles);
    }

    [HttpPut("{id}/placa")]
    public async Task<IActionResult> UpdatePlate([FromBody] UpdatePlateByPlateRequest req, string id)
    {
        // TODO: Implement dados inválidos 400
        if (string.IsNullOrWhiteSpace(req.Placa) || string.IsNullOrWhiteSpace(id))
            return BadRequest(new { mensagem = "Dados inválidos" });
        
        var updated = await motorcycleRepository.UpdatePlateAsync(id, req.Placa);
        
        if (!updated)
            return BadRequest(new { mensagem = "Dados inválidos" });

        return Ok(new { mensagem = "Placa atualizada com sucesso" });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByPlate(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { mensagem = "Dados inválidos" });

        // // TODO: Verify if motorcycle has rentals before deleting
        // if ("hasRentals" == "false")
        //     return BadRequest(new { mensagem = "Dados inválidos" });

        var deleted = await motorcycleRepository.DeleteAsync(id);
        return deleted ? Ok() : BadRequest(new { mensagem = "Dados inválidos" });
    }
}
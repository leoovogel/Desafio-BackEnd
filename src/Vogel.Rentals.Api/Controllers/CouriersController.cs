using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("entregadores")]
public class CouriersController(
    ICourierRepository courierRepository,
    ICourierService courierService,
    ICourierValidator courierValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourierRequest? req)
    {
        var courier = courierValidator.ValidateAndNormalizeCreate(req);

        var saved = await courierRepository.AddAsync(courier);
        return Created($"/couriers/{saved.Identifier}", null);
    }

    [HttpPost("{id}/cnh")]
    public async Task<IActionResult> UploadCnhImage([FromRoute] string id, [FromBody] UpdateCourierCnhImageRequest? req)
    {
        courierValidator.ValidateUploadCnhImage(id, req);

        await courierService.UploadCnhImageAsync(id, req.ImagemCnh);

        return Created($"/couriers/{id}/cnh", null);
    }
}
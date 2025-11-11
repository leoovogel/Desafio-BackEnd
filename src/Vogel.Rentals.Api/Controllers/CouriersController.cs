using Microsoft.AspNetCore.Mvc;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Api.Controllers;

[ApiController]
[Route("entregadores")]
public class CouriersController(ICourierRepository courierRepository, IStorageService storageService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourierRequest? req)
    {
        if (req is null ||
            string.IsNullOrWhiteSpace(req.Identificador) ||
            string.IsNullOrWhiteSpace(req.Nome) ||
            string.IsNullOrWhiteSpace(req.Cnpj) ||
            string.IsNullOrWhiteSpace(req.NumeroCnh) ||
            req.DataNascimento == default ||
            string.IsNullOrWhiteSpace(req.TipoCnh) ||
            !TryParseCnhType(req.TipoCnh, out var cnhType))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var entity = new Courier
        {
            Identifier = req.Identificador,
            Name = req.Nome,
            Cnpj = req.Cnpj,
            BirthDate = req.DataNascimento,
            CnhNumber = req.NumeroCnh,
            CnhType = cnhType,
            CnhImage = null // TODO: Implement after
        };

        var saved = await courierRepository.AddAsync(entity);

        return Created($"/couriers/{saved.Identifier}", null);
    }

    [HttpPost("{id}/cnh")]
    public async Task<IActionResult> UploadCnhImage([FromRoute] string id, [FromBody] UpdateCourierCnhImageRequest? req)
    {
        if (string.IsNullOrWhiteSpace(id) || req is null || string.IsNullOrWhiteSpace(req.ImagemCnh))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var courier = await courierRepository.GetByIdentifierAsync(id);
        if (courier is null)
            return NotFound(new { mensagem = "Dados inválidos" });

        if (!TryDecodeImage(req.ImagemCnh, out var bytes, out var contentType, out var extension))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var fileName = $"{id}_cnh_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
        var storageKey = await storageService.SaveAsync(fileName, bytes);

        await courierRepository.UpdateCnhImageAsync(id, storageKey);

        return Created($"/couriers/{id}/cnh", null);
    }

    private static bool TryParseCnhType(string input, out CnhType cnh)
    {
        var typeUpper = input.Trim().ToUpperInvariant();
        cnh = typeUpper switch
        {
            "A"   => CnhType.A,
            "B"   => CnhType.B,
            "A+B" => CnhType.AB,
            _     => 0
        };

        return cnh != 0;
    }

    private static bool TryDecodeImage(string base64, out byte[] bytes, out string contentType, out string extension)
    {
        bytes = [];
        contentType = string.Empty;
        extension = string.Empty;

        if (string.IsNullOrWhiteSpace(base64))
            return false;

        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch
        {
            return false;
        }

        if (bytes.Length < 4)
            return false;

        // PNG header: 89 50 4E 47 - https://en.wikipedia.org/wiki/List_of_file_signatures
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
        {
            contentType = "image/png";
            extension = ".png";
            return true;
        }

        // BMP header: 42 4D ('B''M') - https://en.wikipedia.org/wiki/List_of_file_signatures
        if (bytes[0] == 0x42 && bytes[1] == 0x4D)
        {
            contentType = "image/bmp";
            extension = ".bmp";
            return true;
        }

        return false;
    }
}
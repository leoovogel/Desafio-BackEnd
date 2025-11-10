using Microsoft.AspNetCore.Mvc;
using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Application.Contracts;
using Mottu.Rentals.Domain.Entities;
using Mottu.Rentals.Domain.Enums;

namespace Mottu.Rentals.Api.Controllers;

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
        
        if (await courierRepository.CnpjExistsAsync(req.Cnpj))
            return Conflict(new { mensagem = "O CNPJ já existe." });
        
        if (await courierRepository.CnhNumberExistsAsync(req.NumeroCnh))
            return Conflict(new { error = "O número CNH já existe" });
        
        var birthDate = DateOnly.FromDateTime(req.DataNascimento);

        var entity = new Courier
        {
            Identificador = req.Identificador,
            Nome = req.Nome,
            Cnpj = req.Cnpj,
            DataNascimento = birthDate,
            NumeroCnh = req.NumeroCnh,
            TipoCnh = cnhType,
            ImagemCnh = null // TODO: Implement after
        };

        var saved = await courierRepository.AddAsync(entity);

        var res = new CourierResponse(
            saved.Identificador,
            saved.Nome,
            saved.Cnpj,
            // TODO: Confirm date format "dd-MM-yyyy"
            saved.DataNascimento.ToString("dd-MM-yyyy"),
            saved.NumeroCnh,
            saved.TipoCnh switch { CnhType.A => "A", CnhType.B => "B", CnhType.AB => "A+B", _ => "A" },
            saved.ImagemCnh
        );

        return Created($"/couriers/{saved.Identificador}", res);
    }

    [HttpPost("{id}/cnh")]
    public async Task<IActionResult> UploadCnhImage([FromRoute] string id, [FromBody] UpdateCourierCnhImageRequest? req)
    {
        if (string.IsNullOrWhiteSpace(id) || req is null || string.IsNullOrWhiteSpace(req.ImagemCnh))
            return BadRequest(new { mensagem = "Dados inválidos" });

        var courier = await courierRepository.GetByIdentificadorAsync(id);
        if (courier is null)
            return NotFound(new { mensagem = "Dados inválidos" });

        if (!TryDecodeImage(req.ImagemCnh, out var bytes, out var contentType, out var extension, out var error))
            return BadRequest(new { mensagem = error });

        var fileName = $"{id}_cnh_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
        var storageKey = await storageService.SaveAsync(fileName, bytes);

        var updatedOk = await courierRepository.UpdateCnhImageAsync(id, storageKey);
        if (!updatedOk)
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Não foi possível atualizar a imagem da CNH." });

        var updated = await courierRepository.GetByIdentificadorAsync(id) ?? courier;

        var res = new CourierResponse(
            updated.Identificador,
            updated.Nome,
            updated.Cnpj,
            updated.DataNascimento.ToString("dd-MM-yyyy"),
            updated.NumeroCnh,
            updated.TipoCnh switch { CnhType.A => "A", CnhType.B => "B", CnhType.AB => "A+B", _ => "A" },
            updated.ImagemCnh
        );

        return Ok(res);
    }

    private static bool TryParseCnhType(string input, out CnhType cnh)
    {
        var v = input.Trim().ToUpperInvariant();
        cnh = v switch
        {
            "A"   => CnhType.A,
            "B"   => CnhType.B,
            "A+B" => CnhType.AB,
            _     => 0
        };

        return cnh != 0;
    }

    private static bool TryDecodeImage(string base64, out byte[] bytes, out string contentType, out string extension, out string error)
    {
        bytes = [];
        contentType = string.Empty;
        extension = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(base64))
        {
            error = "Imagem inválida.";
            return false;
        }

        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch
        {
            error = "Base64 inválido.";
            return false;
        }

        if (bytes.Length < 4)
        {
            error = "Imagem muito pequena.";
            return false;
        }

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

        error = "Formato de imagem não suportado. Use PNG ou BMP.";
        return false;
    }
}
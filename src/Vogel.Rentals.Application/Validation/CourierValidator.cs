using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Validation;

public class CourierValidator : ICourierValidator
{
    public Courier ValidateAndNormalizeCreate(CreateCourierRequest? req)
    {
        if (req is null ||
            string.IsNullOrWhiteSpace(req.Identificador) ||
            string.IsNullOrWhiteSpace(req.Nome) ||
            string.IsNullOrWhiteSpace(req.Cnpj) ||
            string.IsNullOrWhiteSpace(req.NumeroCnh) ||
            req.DataNascimento == default ||
            string.IsNullOrWhiteSpace(req.TipoCnh) ||
            !TryParseCnhType(req.TipoCnh, out var cnhType))
        {
            throw new BusinessRuleException();
        }
        
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

        return entity;
    }

    public void ValidateUploadCnhImage(string id, UpdateCourierCnhImageRequest? req)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            req is null ||
            string.IsNullOrWhiteSpace(req.ImagemCnh))
        {
            throw new BusinessRuleException();
        }
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
}
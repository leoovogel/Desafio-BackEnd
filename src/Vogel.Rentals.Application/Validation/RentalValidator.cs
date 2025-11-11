using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Validation;

public class RentalValidator : IRentalValidator
{
    public void ValidateCreate(CreateRentalRequest? req)
    {
        if (req is null ||
            string.IsNullOrWhiteSpace(req.EntregadorId) ||
            string.IsNullOrWhiteSpace(req.MotoId) ||
            req.DataInicio == default ||
            req.DataTermino == default ||
            req.DataPrevisaoTermino == default)
        {
            throw new BusinessRuleException();
        }
    }

    public Guid ValidateAndParseGetById(string id)
    {
        if (!Guid.TryParse(id, out var idGuid) || idGuid == Guid.Empty)
            throw new BusinessRuleException();
        
        return idGuid;
    }

    public void ValidadeRentalReturn(Guid id, CalculateRentalTotalRequest? req)
    {
        if (id == Guid.Empty || req is null || req.DataDevolucao == default)
            throw new BusinessRuleException();
    }
}
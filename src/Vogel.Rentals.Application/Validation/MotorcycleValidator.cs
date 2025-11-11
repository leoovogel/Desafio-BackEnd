using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Validation;

public class MotorcycleValidator : IMotorcycleValidator
{
    public void ValidateCreate(CreateMotorcycleRequest req)
    {
        if (req is null ||
            string.IsNullOrWhiteSpace(req.Identificador) ||
            string.IsNullOrWhiteSpace(req.Modelo) ||
            string.IsNullOrWhiteSpace(req.Placa) ||
            req.Ano <= 0)
        {
            throw new BusinessRuleException();
        }
    }

    public void ValidateUpdatePlate(string id, UpdatePlateByPlateRequest req)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            req is null ||
            string.IsNullOrWhiteSpace(req.Placa))
        {
            throw new BusinessRuleException();
        }
    }

    public void ValidateSearchById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new BusinessRuleException();
    }
}
using Vogel.Rentals.Application.Contracts;

namespace Vogel.Rentals.Application.Abstractions;

public interface IRentalValidator
{
    void ValidateCreate(CreateRentalRequest? req);
    Guid ValidateAndParseGetById(string id);
    void ValidadeRentalReturn(Guid id, CalculateRentalTotalRequest? req);
}
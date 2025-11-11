using Vogel.Rentals.Application.Contracts;

namespace Vogel.Rentals.Application.Abstractions;

public interface IMotorcycleValidator
{
    void ValidateCreate(CreateMotorcycleRequest req);
    void ValidateUpdatePlate(string id, UpdatePlateByPlateRequest req);
    void ValidateSearchById(string id);
}
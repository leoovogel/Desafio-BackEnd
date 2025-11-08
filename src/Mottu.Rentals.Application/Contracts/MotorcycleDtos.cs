namespace Mottu.Rentals.Application.Contracts;

public record CreateMotorcycleRequest(string Identifier, int Year, string Model, string Plate);
public record UpdatePlateByPlateRequest(string CurrentPlate, string NewPlate);

public record MotorcycleResponse(Guid Id, string Identifier, int Year, string Model, string Plate);

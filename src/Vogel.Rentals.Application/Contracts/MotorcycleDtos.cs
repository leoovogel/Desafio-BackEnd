namespace Vogel.Rentals.Application.Contracts;

public record CreateMotorcycleRequest(string Identificador, int Ano, string Modelo, string Placa);
public record UpdatePlateByPlateRequest(string Placa);

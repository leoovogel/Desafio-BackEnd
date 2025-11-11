using Mottu.Rentals.Domain.Enums;

namespace Mottu.Rentals.Domain.Entities;

public class Courier
{
    public required string Identificador { get; set; }
    public required string Nome { get; set; }
    public required string Cnpj { get; set; }
    public DateTime DataNascimento { get; set; }
    public required string NumeroCnh { get; set; }
    public CnhType TipoCnh { get; set; }
    public string? ImagemCnh { get; set; }
}
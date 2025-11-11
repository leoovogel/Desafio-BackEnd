using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Domain.Entities;

public class Courier
{
    public required string Identifier { get; set; }
    public required string Name { get; set; }
    public required string Cnpj { get; set; }
    public DateTime BirthDate { get; set; }
    public required string CnhNumber { get; set; }
    public CnhType CnhType { get; set; }
    public string? CnhImage { get; set; }
}
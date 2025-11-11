namespace Vogel.Rentals.Domain.Entities;

public class Motorcycle
{
    public required string Identifier { get; set; }
    public required int Year { get; set; }
    public required string Model { get; set; }
    public required string Plate { get; set; }
    
    public ICollection<Rental>? Rentals { get; set; }
}
namespace Vogel.Rentals.Domain.Entities;

public class MotorcycleNotification
{
    public string Identifier { get; set; }
    public int Year { get; set; }
    public string Model { get; set; }
    public string Plate { get; set; }
    public DateTime CreatedAt { get; set; }
}
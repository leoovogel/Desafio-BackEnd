using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Domain.Entities;

public class Rental
{
    public Guid Identifier { get; set; } = Guid.NewGuid();
    public required string CourierId { get; set; }
    public required string MotorcycleId { get; set; }
    
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required DateTime ExpectedEndDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public required RentalPlan Plan { get; set; }
    public required decimal DailyRate { get; set; }
}
using Mottu.Rentals.Domain.Enums;

namespace Mottu.Rentals.Domain.Entities;

public class Rental
{
    public Guid Identifier { get; set; } = Guid.NewGuid();
    public required string CourierId { get; set; }
    public required string MotorcycleId { get; set; }
    
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public required DateOnly ExpectedEndDate { get; set; }

    public required RentalPlan Plan { get; set; }
    public required decimal DailyRate { get; set; }
}
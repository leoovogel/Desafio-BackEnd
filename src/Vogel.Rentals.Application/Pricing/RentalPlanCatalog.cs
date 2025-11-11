using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Application.Pricing;

public static class RentalPlanCatalog
{
    private static readonly Dictionary<int, (RentalPlan Plan, decimal DailyRate)> RentalPlans =
        new()
        {
            { 7,  (RentalPlan.Days7, 30m) },
            { 15, (RentalPlan.Days15, 28m) },
            { 30, (RentalPlan.Days30, 22m) },
            { 45, (RentalPlan.Days45, 20m) },
            { 50, (RentalPlan.Days50, 18m) },
        };

    public static bool TryGet(int days, out RentalPlan plan, out decimal dailyRate)
    {
        if (!RentalPlans.TryGetValue(days, out var info))
        {
            plan = default;
            dailyRate = 0;
            return false;
        }

        plan = info.Plan;
        dailyRate = info.DailyRate;
        return true;
    }
    
    public static decimal CalculateTotal(Rental rental, DateTime actualReturnDate)
    {
        ArgumentNullException.ThrowIfNull(rental);

        var planDays = (int)rental.Plan;
        
        if (actualReturnDate <= rental.ExpectedEndDate)
        {
            var usedDays = (actualReturnDate.Date - rental.StartDate.Date).Days + 1;            
            var usedValue = usedDays * rental.DailyRate;

            var unusedDays = planDays - usedDays;

            if (actualReturnDate.Date >= rental.ExpectedEndDate.Date || unusedDays <= 0)
                return usedValue;

            var penaltyRate = rental.Plan switch
            {
                RentalPlan.Days7  => 0.20m,
                RentalPlan.Days15 => 0.40m,
                _                 => 0m
            };

            var unusedValue = unusedDays * rental.DailyRate;
            var penalty = unusedValue * penaltyRate;

            return usedValue + penalty;
        }

        var baseValue = planDays * rental.DailyRate;
        var extraDays = (actualReturnDate.Date - rental.ExpectedEndDate.Date).Days;

        var extraCharge = extraDays * 50m;
        return baseValue + extraCharge;
    }
}
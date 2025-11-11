using Mottu.Rentals.Domain.Enums;

namespace Mottu.Rentals.Application.Pricing;

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
}
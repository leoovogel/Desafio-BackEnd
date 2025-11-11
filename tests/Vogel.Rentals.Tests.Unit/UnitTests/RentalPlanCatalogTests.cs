using FluentAssertions;
using Vogel.Rentals.Application.Pricing;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class RentalPlanCatalogTests
{
    [Fact]
    public void CalculateTotal_Should_ReturnOnlyDailyValue_WhenReturnedOnExpectedDate()
    {
        // Arrange
        var rental = new Rental
        {
            Identifier      = Guid.NewGuid(),
            CourierId       = "entregador001",
            MotorcycleId    = "moto001",
            Plan            = RentalPlan.Days7,
            DailyRate       = 30m,
            StartDate       = new DateTime(2025, 1, 1),
            ExpectedEndDate = new DateTime(2025, 1, 7),
            EndDate         = new DateTime(2025, 1, 7)
        };

        var returnDate = new DateTime(2025, 1, 7);

        // Act
        var total = RentalPlanCatalog.CalculateTotal(rental, returnDate);

        // Assert
        total.Should().Be(7 * 30m);
    }

    [Fact]
    public void CalculateTotal_Should_ApplyPenalty_WhenReturnedEarlier_On7DaysPlan()
    {
        // Arrange
        var rental = new Rental
        {
            Identifier      = Guid.NewGuid(),
            CourierId       = "entregador001",
            MotorcycleId    = "moto001",
            Plan            = RentalPlan.Days7,
            DailyRate       = 30m,
            StartDate       = new DateTime(2025, 1, 1),
            ExpectedEndDate = new DateTime(2025, 1, 7),
            EndDate         = new DateTime(2025, 1, 7)
        };

        var returnDate = new DateTime(2025, 1, 5);

        // Act
        var total = RentalPlanCatalog.CalculateTotal(rental, returnDate);

        // Assert
        total.Should().Be(162m);
    }

    [Fact]
    public void CalculateTotal_Should_ApplyExtraDailyFee_WhenReturnedAfterExpectedDate()
    {
        // Arrange
        var rental = new Rental
        {
            Identifier      = Guid.NewGuid(),
            CourierId       = "entregador001",
            MotorcycleId    = "moto001",
            Plan            = RentalPlan.Days7,
            DailyRate       = 30m,
            StartDate       = new DateTime(2025, 1, 1),
            ExpectedEndDate = new DateTime(2025, 1, 7),
            EndDate         = new DateTime(2025, 1, 7)
        };

        var returnDate = new DateTime(2025, 1, 9);

        // Act
        var total = RentalPlanCatalog.CalculateTotal(rental, returnDate);

        // Assert
        total.Should().Be(310m);
    }
}
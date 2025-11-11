using FluentAssertions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Validation;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class RentalValidatorTests
{
    [Fact]
    public void ValidateAndNormalizeCreate_Should_NotThrow_ForValidRequest()
    {
        // Arrange
        var validator = new RentalValidator();
        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto001",
            DataInicio: new DateTime(2025, 1, 1),
            DataTermino: new DateTime(2025, 1, 7),
            DataPrevisaoTermino: new DateTime(2025, 1, 7),
            Plano: 7);

        // Act
        var act = () => validator.ValidateCreate(req);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateAndNormalizeCreate_Should_Throw_WhenRequiredFieldsMissing()
    {
        // Arrange
        var validator = new RentalValidator();
        var req = new CreateRentalRequest(
            EntregadorId: "",
            MotoId: "",
            DataInicio: default,
            DataTermino: default,
            DataPrevisaoTermino: default,
            Plano: 0);

        // Act
        var act = () => validator.ValidateCreate(req);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateAndParseGetById_Should_Throw_WhenIdIsInvalid(string? id)
    {
        // Arrange
        var validator = new RentalValidator();

        // Act
        var act = () => validator.ValidateAndParseGetById(id!);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }

    [Fact]
    public void ValidateAndParseGetById_Should_ReturnGuid_WhenIdIsValidGuid()
    {
        // Arrange
        var validator = new RentalValidator();
        var id = Guid.NewGuid().ToString();

        // Act
        var guid = validator.ValidateAndParseGetById(id);

        // Assert
        guid.Should().NotBeEmpty();
        guid.ToString().Should().Be(id);
    }

    [Fact]
    public void ValidadeRentalReturn_Should_Throw_WhenReturnDateMissing()
    {
        // Arrange
        var validator = new RentalValidator();
        var id = Guid.NewGuid();
        var req = new CalculateRentalTotalRequest(DataDevolucao: default);

        // Act
        var act = () => validator.ValidadeRentalReturn(id, req);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }
}
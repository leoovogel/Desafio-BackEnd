using FluentAssertions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Validation;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class MotorcycleValidatorTests
{
    [Fact]
    public void ValidateCreate_Should_NotThrow_ForValidRequest()
    {
        // Arrange
        var validator = new MotorcycleValidator();
        var req = new CreateMotorcycleRequest(
            Identificador: "moto001",
            Ano: 2024,
            Modelo: "Honda CG 160",
            Placa: "ABC1D23");

        // Act
        var act = () => validator.ValidateCreate(req);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCreate_Should_ThrowBusinessRuleException_WhenDataInvalid()
    {
        // Arrange
        var validator = new MotorcycleValidator();
        var req = new CreateMotorcycleRequest(
            Identificador: "",
            Ano: 0,
            Modelo: "",
            Placa: "");

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
    public void ValidateSearchById_Should_ThrowBusinessRuleException_WhenIdInvalid(string? id)
    {
        // Arrange
        var validator = new MotorcycleValidator();

        // Act
        var act = () => validator.ValidateSearchById(id!);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }

    [Fact]
    public void ValidateUpdatePlate_Should_ThrowBusinessRuleException_WhenPlateInvalid()
    {
        // Arrange
        var validator = new MotorcycleValidator();
        var request   = new UpdatePlateByPlateRequest(Placa: "");

        // Act
        var act = () => validator.ValidateUpdatePlate("moto001", request);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }
}
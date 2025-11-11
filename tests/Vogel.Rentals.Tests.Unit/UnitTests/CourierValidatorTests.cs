using FluentAssertions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Validation;
using Vogel.Rentals.Domain.Enums;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class CourierValidatorTests
{
    [Fact]
    public void ValidateAndNormalizeCreate_Should_ReturnCourier_ForValidRequest()
    {
        // Arrange
        var validator = new CourierValidator();
        var req = new CreateCourierRequest(
            Identificador: "entregador001",
            Nome: "João da Silva",
            Cnpj: "12345678000190",
            DataNascimento: new DateTime(1990, 1, 1),
            NumeroCnh: "CNH123456",
            TipoCnh: "A",
            ImagemCnh: null);

        // Act
        var courier = validator.ValidateAndNormalizeCreate(req);

        // Assert
        courier.Identifier.Should().Be("entregador001");
        courier.Name.Should().Be("João da Silva");
        courier.Cnpj.Should().Be("12345678000190");
        courier.CnhNumber.Should().Be("CNH123456");
        courier.CnhType.Should().Be(CnhType.A);
    }

    [Fact]
    public void ValidateAndNormalizeCreate_Should_ThrowBusinessRuleException_WhenDataInvalid()
    {
        // Arrange
        var validator = new CourierValidator();
        var req = new CreateCourierRequest(
            Identificador: "",
            Nome: "",
            Cnpj: "",
            DataNascimento: default,
            NumeroCnh: "",
            TipoCnh: "",
            ImagemCnh: null);

        // Act
        var act = () => validator.ValidateAndNormalizeCreate(req);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }

    [Fact]
    public void ValidateAndNormalizeCreate_Should_ThrowBusinessRuleException_WhenCnhTypeInvalid()
    {
        // Arrange
        var validator = new CourierValidator();
        var req = new CreateCourierRequest(
            Identificador: "entregador002",
            Nome: "Maria",
            Cnpj: "12345678000191",
            DataNascimento: new DateTime(1990, 1, 1),
            NumeroCnh: "CNH999999",
            TipoCnh: "C",
            ImagemCnh: null);

        // Act
        var act = () => validator.ValidateAndNormalizeCreate(req);

        // Assert
        act.Should()
           .Throw<BusinessRuleException>()
           .Which.Message.Should().Be("Dados inválidos");
    }
}
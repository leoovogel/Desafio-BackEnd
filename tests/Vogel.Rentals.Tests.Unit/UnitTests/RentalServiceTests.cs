using FluentAssertions;
using Moq;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Application.Services;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Enums;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class RentalServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_ThrowBusinessRule_WhenStartDateAfterExpectedEndDate()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto001",
            DataInicio: new DateTime(2025, 1, 10),
            DataTermino: new DateTime(2025, 1, 12),
            DataPrevisaoTermino: new DateTime(2025, 1, 5),
            Plano: 7);

        // Act
        var act = () => service.CreateAsync(req);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowBusinessRule_WhenPlanIsInvalid()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto001",
            DataInicio: new DateTime(2025, 1, 1),
            DataTermino: new DateTime(2025, 1, 7),
            DataPrevisaoTermino: new DateTime(2025, 1, 7),
            Plano: 99);

        // Act
        var act = () => service.CreateAsync(req);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowBusinessRule_WhenCourierDoesNotHaveCategoryA()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto001",
            DataInicio: new DateTime(2025, 1, 1),
            DataTermino: new DateTime(2025, 1, 7),
            DataPrevisaoTermino: new DateTime(2025, 1, 7),
            Plano: 7);

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador001"))
                   .ReturnsAsync(new Courier
                   {
                       Identifier = "entregador001",
                       Name = "João",
                       CnhType = CnhType.B,
                       Cnpj = "12345678000190",
                       CnhNumber = "CNH123456"
                   });

        // Act
        var act = () => service.CreateAsync(req);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowBusinessRule_WhenMotorcycleNotFound()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto-nao-existe",
            DataInicio: new DateTime(2025, 1, 1),
            DataTermino: new DateTime(2025, 1, 7),
            DataPrevisaoTermino: new DateTime(2025, 1, 7),
            Plano: 7);

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador001"))
                   .ReturnsAsync(new Courier
                   {
                       Identifier = "entregador001",
                       Name = "João",
                       CnhType = CnhType.A,
                       Cnpj = "12345678000190",
                       CnhNumber = "CNH123456"
                   });

        motoRepo.Setup(r => r.GetByIdAsync("moto-nao-existe"))
                .ReturnsAsync((Motorcycle?)null);

        // Act
        var act = () => service.CreateAsync(req);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task CreateAsync_Should_CreateRental_WhenDataIsValid()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var req = new CreateRentalRequest(
            EntregadorId: "entregador001",
            MotoId: "moto001",
            DataInicio: new DateTime(2025, 1, 1),
            DataTermino: new DateTime(2025, 1, 7),
            DataPrevisaoTermino: new DateTime(2025, 1, 7),
            Plano: 7);

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador001"))
                   .ReturnsAsync(new Courier
                   {
                       Identifier = "entregador001",
                       Name = "João",
                       CnhType = CnhType.A,
                       Cnpj = "12345678000190",
                       CnhNumber = "CNH123456"
                   });

        motoRepo.Setup(r => r.GetByIdAsync("moto001"))
                .ReturnsAsync(new Motorcycle
                {
                    Identifier = "moto001",
                    Model      = "Honda CG",
                    Plate      = "ABC1D23",
                    Year       = 2024
                });

        rentalRepo.Setup(r => r.AddAsync(It.IsAny<Rental>()))
                  .ReturnsAsync((Rental r) =>
                  {
                      r.Identifier = Guid.NewGuid();
                      return r;
                  });

        // Act
        var rental = await service.CreateAsync(req);

        // Assert
        rental.Should().NotBeNull();
        rental.Identifier.Should().NotBe(Guid.Empty);
        rental.CourierId.Should().Be("entregador001");
        rental.MotorcycleId.Should().Be("moto001");
        rental.Plan.Should().Be(RentalPlan.Days7);
        rental.DailyRate.Should().Be(30m);

        rentalRepo.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_ThrowNotFound_WhenRentalDoesNotExist()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var id = Guid.NewGuid();

        rentalRepo.Setup(r => r.GetByIdAsync(id))
                  .ReturnsAsync((Rental?)null);

        // Act
        var act = () => service.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("Locação não encontrada");
    }

    [Fact]
    public async Task RentalReturnAsync_Should_ThrowBusinessRule_WhenRentalDoesNotExist()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var id = Guid.NewGuid();

        rentalRepo.Setup(r => r.GetByIdAsync(id))
                  .ReturnsAsync((Rental?)null);

        // Act
        var act = () => service.RentalReturnAsync(id, DateTime.UtcNow);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task RentalReturnAsync_Should_ThrowBusinessRule_WhenReturnDateBeforeStart()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var id = Guid.NewGuid();

        var rental = new Rental
        {
            Identifier      = id,
            CourierId       = "entregador001",
            MotorcycleId    = "moto001",
            Plan            = RentalPlan.Days7,
            DailyRate       = 30m,
            StartDate       = new DateTime(2025, 1, 10),
            ExpectedEndDate = new DateTime(2025, 1, 17),
            EndDate         = new DateTime(2025, 1, 17)
        };

        rentalRepo.Setup(r => r.GetByIdAsync(id))
                  .ReturnsAsync(rental);

        var returnDate = new DateTime(2025, 1, 5);

        // Act
        var act = () => service.RentalReturnAsync(id, returnDate);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task RentalReturnAsync_Should_UpdateRental_And_ReturnTotal()
    {
        // Arrange
        var motoRepo    = new Mock<IMotorcycleRepository>();
        var courierRepo = new Mock<ICourierRepository>();
        var rentalRepo  = new Mock<IRentalRepository>();

        var service = new RentalService(motoRepo.Object, courierRepo.Object, rentalRepo.Object);

        var id = Guid.NewGuid();

        var rental = new Rental
        {
            Identifier      = id,
            CourierId       = "entregador001",
            MotorcycleId    = "moto001",
            Plan            = RentalPlan.Days7,
            DailyRate       = 30m,
            StartDate       = new DateTime(2025, 1, 1),
            ExpectedEndDate = new DateTime(2025, 1, 7),
            EndDate         = new DateTime(2025, 1, 7)
        };

        rentalRepo.Setup(r => r.GetByIdAsync(id))
                  .ReturnsAsync(rental);

        rentalRepo.Setup(r => r.UpdateAsync(It.IsAny<Rental>()))
                  .ReturnsAsync((Rental r) => r);

        var returnDate = new DateTime(2025, 1, 7);

        // Act
        var response = await service.RentalReturnAsync(id, returnDate);

        // Assert
        response.ValorTotal.Should().Be(210m);
        response.Mensagem.Should().Be("Data de devolução informada com sucesso");

        rental.ReturnDate.Should().Be(returnDate);

        rentalRepo.Verify(r => r.UpdateAsync(It.Is<Rental>(x => x.Identifier == id && x.ReturnDate == returnDate)),
                          Times.Once);
    }
}
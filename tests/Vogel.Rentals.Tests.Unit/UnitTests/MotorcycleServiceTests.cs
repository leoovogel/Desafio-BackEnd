using FluentAssertions;
using Moq;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Services;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class MotorcycleServiceTests
{
    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFound_WhenMotorcycleDoesNotExist()
    {
        // Arrange
        var motoRepo   = new Mock<IMotorcycleRepository>();
        var rentalRepo = new Mock<IRentalRepository>();
        var eventPublisher = new Mock<IMotorcycleEventPublisher>();

        motoRepo.Setup(r => r.GetByIdAsync("moto-nao-existe"))
                .ReturnsAsync((Motorcycle?)null);

        var service = new MotorcycleService(motoRepo.Object, rentalRepo.Object, eventPublisher.Object);

        // Act
        var act = () => service.DeleteAsync("moto-nao-existe");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("Moto não encontrada");
    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowBusinessRule_WhenHasRentals()
    {
        // Arrange
        var motoRepo   = new Mock<IMotorcycleRepository>();
        var rentalRepo = new Mock<IRentalRepository>();
        var eventPublisher = new Mock<IMotorcycleEventPublisher>();

        motoRepo.Setup(r => r.GetByIdAsync("moto001"))
                .ReturnsAsync(new Motorcycle
                {
                    Identifier = "moto001",
                    Model      = "Honda CG 160",
                    Plate      = "ABC1D23",
                    Year       = 2024
                });

        rentalRepo.Setup(r => r.HasRentalsForMotorcycleAsync("moto001"))
                  .ReturnsAsync(true);

        var service = new MotorcycleService(motoRepo.Object, rentalRepo.Object, eventPublisher.Object);

        // Act
        var act = () => service.DeleteAsync("moto001");

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task DeleteAsync_Should_CallRepositoryDelete_WhenNoRentals()
    {
        // Arrange
        var motoRepo   = new Mock<IMotorcycleRepository>();
        var rentalRepo = new Mock<IRentalRepository>();
        var eventPublisher = new Mock<IMotorcycleEventPublisher>();

        motoRepo.Setup(r => r.GetByIdAsync("moto001"))
                .ReturnsAsync(new Motorcycle
                {
                    Identifier = "moto001",
                    Model      = "Honda CG 160",
                    Plate      = "ABC1D23",
                    Year       = 2024
                });

        rentalRepo.Setup(r => r.HasRentalsForMotorcycleAsync("moto001"))
                  .ReturnsAsync(false);

        var service = new MotorcycleService(motoRepo.Object, rentalRepo.Object, eventPublisher.Object);

        // Act
        await service.DeleteAsync("moto001");

        // Assert
        motoRepo.Verify(r => r.DeleteAsync("moto001"), Times.Once);
    }
}
using FluentAssertions;
using Moq;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Services;
using Vogel.Rentals.Domain.Entities;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Tests.Unit.UnitTests;

public class CourierServiceTests
{
    [Fact]
    public async Task UploadCnhImageAsync_Should_ThrowBusinessRule_WhenCourierNotFound()
    {
        // Arrange
        var courierRepo  = new Mock<ICourierRepository>();
        var storage      = new Mock<IStorageService>();

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador-nao-existe"))
                   .ReturnsAsync((Courier?)null);

        var service = new CourierService(courierRepo.Object, storage.Object);

        // Act
        var act = () => service.UploadCnhImageAsync("entregador-nao-existe", "qualquer-coisa");

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task UploadCnhImageAsync_Should_ThrowBusinessRule_WhenBase64IsInvalid()
    {
        // Arrange
        var courierRepo  = new Mock<ICourierRepository>();
        var storage      = new Mock<IStorageService>();

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador001"))
                   .ReturnsAsync(new Courier
                   {
                       Identifier = "entregador001",
                       Name = "João",
                       Cnpj = "12345678000190",
                       CnhNumber = "CNH123456",
                   });

        var service = new CourierService(courierRepo.Object, storage.Object);

        const string invalidBase64 = "isso-nao-e-base64";

        // Act
        var act = () => service.UploadCnhImageAsync("entregador001", invalidBase64);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
                 .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task UploadCnhImageAsync_Should_SaveImageAndUpdateCourier_WhenPngIsValid()
    {
        // Arrange
        var courierRepo  = new Mock<ICourierRepository>();
        var storage      = new Mock<IStorageService>();

        courierRepo.Setup(r => r.GetByIdentifierAsync("entregador001"))
                   .ReturnsAsync(new Courier
                   {
                       Identifier = "entregador001",
                       Name = "João",
                       Cnpj = "12345678000190",
                       CnhNumber = "CNH123456",
                   });

        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x00, 0x01, 0x02, 0x03 };
        var base64   = Convert.ToBase64String(pngBytes);

        storage.Setup(s => s.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>()))
            .ReturnsAsync("storage-key-123");

        var service = new CourierService(courierRepo.Object, storage.Object);

        // Act
        await service.UploadCnhImageAsync("entregador001", base64);

        // Assert
        storage.Verify(s => s.SaveAsync(
            It.Is<string>(fileName =>
                fileName.StartsWith("entregador001_cnh_") &&
                fileName.EndsWith(".png")),
            It.Is<byte[]>(b => b.Length == pngBytes.Length)),
            Times.Once);

        courierRepo.Verify(r => r.UpdateCnhImageAsync(
                "entregador001",
                "storage-key-123"),
            Times.Once);
    }
}
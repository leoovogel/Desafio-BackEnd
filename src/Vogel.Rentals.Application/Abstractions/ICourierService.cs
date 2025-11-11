namespace Vogel.Rentals.Application.Abstractions;

public interface ICourierService
{
    Task UploadCnhImageAsync(string courierId, string base64Image);
}
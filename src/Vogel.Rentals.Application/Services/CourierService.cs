using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Application.Services;

public class CourierService(
    ICourierRepository courierRepository,
    IStorageService storageService
    ) : ICourierService
{
    public async Task UploadCnhImageAsync(string courierId, string base64Image)
    {
        var courier = await courierRepository.GetByIdentifierAsync(courierId);
        if (courier is null)
            throw new BusinessRuleException();

        if (!TryDecodeImage(base64Image, out var bytes, out var contentType, out var extension))
            throw new BusinessRuleException();

        var fileName = $"{courierId}_cnh_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
        var storageKey = await storageService.SaveAsync(fileName, bytes);

        await courierRepository.UpdateCnhImageAsync(courierId, storageKey);
    }
    
    private static bool TryDecodeImage(string base64, out byte[] bytes, out string contentType, out string extension)
    {
        bytes = [];
        contentType = string.Empty;
        extension = string.Empty;

        if (string.IsNullOrWhiteSpace(base64))
            return false;

        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch
        {
            return false;
        }

        if (bytes.Length < 4)
            return false;

        // PNG header: 89 50 4E 47 - https://en.wikipedia.org/wiki/List_of_file_signatures
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
        {
            contentType = "image/png";
            extension = ".png";
            return true;
        }

        // BMP header: 42 4D ('B''M') - https://en.wikipedia.org/wiki/List_of_file_signatures
        if (bytes[0] == 0x42 && bytes[1] == 0x4D)
        {
            contentType = "image/bmp";
            extension = ".bmp";
            return true;
        }

        return false;
    }
}
using Mottu.Rentals.Application.Abstractions;

namespace Mottu.Rentals.Infrastructure.Local;

public class LocalStorageService : IStorageService
{
    private readonly string _root = Path.Combine(AppContext.BaseDirectory, "storage", "cnh-images");

    public async Task<string> SaveAsync(string fileName, byte[] content)
    {
        Directory.CreateDirectory(_root);

        var fullPath = Path.Combine(_root, fileName);

        await File.WriteAllBytesAsync(fullPath, content);

        return fileName;
    }
}
namespace Mottu.Rentals.Application.Abstractions;

public interface IStorageService
{
    Task<string> SaveAsync(string fileName, byte[] content);
}
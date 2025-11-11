using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface ICourierRepository
{
    Task<Courier> AddAsync(Courier courier);
    Task<bool> CnpjExistsAsync(string cnpj);
    Task<bool> CnhNumberExistsAsync(string cnhNumber);
    Task<Courier?> GetByIdentificadorAsync(string identificador);
    Task<bool> UpdateCnhImageAsync(string identificador, string imageUrl);
}
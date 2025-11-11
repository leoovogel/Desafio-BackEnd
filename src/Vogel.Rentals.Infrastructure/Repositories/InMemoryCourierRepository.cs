using System.Collections.Concurrent;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Infrastructure.InMemory;

public class InMemoryCourierRepository : ICourierRepository
{
    private readonly ConcurrentDictionary<string, Courier> _store = new();
    private readonly ConcurrentDictionary<string, string> _cnpjs = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _cnhs  = new(StringComparer.Ordinal);

    private static string DigitsOnly(string s) => new(s.Where(char.IsDigit).ToArray());

    public Task<bool> CnpjExistsAsync(string cnpj) => Task.FromResult(_cnpjs.ContainsKey(DigitsOnly(cnpj)));

    public Task<bool> CnhNumberExistsAsync(string cnhNumber) => Task.FromResult(_cnhs.ContainsKey(DigitsOnly(cnhNumber)));

    public Task<Courier?> GetByIdAsync(string id) => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<Courier?> GetByCnpjAsync(string cnpj)
    {
        var key = DigitsOnly(cnpj);

        return !_cnpjs.TryGetValue(key, out var id)
            ? Task.FromResult<Courier?>(null)
            : Task.FromResult(_store.GetValueOrDefault(id));
    }

    public Task<Courier> AddAsync(Courier courier)
    {
        var cnpjKey = DigitsOnly(courier.Cnpj);
        var cnhKey  = DigitsOnly(courier.NumeroCnh);

        var id = courier.Identificador.Trim();

        var normalized = new Courier
        {
            Identificador = id,
            Nome = courier.Nome.Trim(),
            Cnpj = cnpjKey,
            DataNascimento = courier.DataNascimento,
            NumeroCnh = cnhKey,
            TipoCnh = courier.TipoCnh,
            ImagemCnh = courier.ImagemCnh
        };

        if (!_cnpjs.TryAdd(cnpjKey, id))
            throw new InvalidOperationException("cnpj already exists");

        if (!_cnhs.TryAdd(cnhKey, id))
        {
            _cnpjs.TryRemove(cnpjKey, out _);
            throw new InvalidOperationException("cnh number already exists");
        }

        _store[id] = normalized;
        return Task.FromResult(normalized);
    }
    
    public Task<Courier?> GetByIdentificadorAsync(string identificador)
    {
        var found = _store.Values.FirstOrDefault(c => string.Equals(c.Identificador, identificador));
        return Task.FromResult(found);
    }

    public Task<bool> UpdateCnhImageAsync(string identificador, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(imageUrl))
            return Task.FromResult(false);

        var existing = _store.Values.FirstOrDefault(c => string.Equals(c.Identificador, identificador));

        if (existing is null)
            return Task.FromResult(false);

        var updated = new Courier
        {
            Identificador = existing.Identificador,
            Nome = existing.Nome,
            Cnpj = existing.Cnpj,
            DataNascimento = existing.DataNascimento,
            NumeroCnh = existing.NumeroCnh,
            TipoCnh = existing.TipoCnh,
            ImagemCnh = imageUrl
        };

        _store[existing.Identificador] = updated;
        return Task.FromResult(true);
    }
}
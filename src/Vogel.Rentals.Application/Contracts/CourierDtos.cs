using System.Text.Json.Serialization;

namespace Vogel.Rentals.Application.Contracts;

public record CreateCourierRequest(
    string Identificador,
    string Nome,
    string Cnpj,
    [property: JsonPropertyName("data_nascimento")] DateTime DataNascimento,
    [property: JsonPropertyName("numero_cnh")] string NumeroCnh,
    [property: JsonPropertyName("tipo_cnh")] string TipoCnh,
    [property: JsonPropertyName("imagem_cnh")] string? ImagemCnh
);

public record UpdateCourierCnhImageRequest([property: JsonPropertyName("imagem_cnh")] string ImagemCnh);

public record CourierResponse(string Identificador, string Nome, string Cnpj, string DataNascimento, string NumeroCnh, string TipoCnh, string? ImagemCnh);
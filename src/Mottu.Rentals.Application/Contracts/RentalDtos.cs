using System.Text.Json.Serialization;

namespace Mottu.Rentals.Application.Contracts;

public record CreateRentalRequest(
    [property: JsonPropertyName("entregador_id")]           string EntregadorId,
    [property: JsonPropertyName("moto_id")]                 string MotoId,
    [property: JsonPropertyName("data_inicio")]             DateTime DataInicio,
    [property: JsonPropertyName("data_termino")]            DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")]   DateTime DataPrevisaoTermino,
                                                            int Plano
);

public record CreateRentalResponse(
                                                            Guid Identificador,
    [property: JsonPropertyName("entregador_id")]           string EntregadorId,
    [property: JsonPropertyName("moto_id")]                 string MotoId,
    [property: JsonPropertyName("data_inicio")]             DateTime DataInicio,
    [property: JsonPropertyName("data_termino")]            DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")]   DateTime DataPrevisaoTermino,
                                                            int Plano,
    [property: JsonPropertyName("valor_diaria")]            decimal ValorDiaria
);

public record RentalResponse(
    Guid Identificador,
    [property: JsonPropertyName("valor_diaria")]            decimal ValorDiaria,
    [property: JsonPropertyName("entregador_id")]           string EntregadorId,
    [property: JsonPropertyName("moto_id")]                 string MotoId,
    [property: JsonPropertyName("data_inicio")]             DateTime DataInicio,
    [property: JsonPropertyName("data_termino")]            DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")]   DateTime DataPrevisaoTermino,
    [property: JsonPropertyName("data_devolucao")]          DateTime DataDevolucao
);

public record CalculateRentalTotalRequest(
    [property: JsonPropertyName("data_devolucao")] DateTime DataDevolucao
);

public record RentalTotalValueResponse(
    [property: JsonPropertyName("valor_total")] decimal ValorTotal,
                                                string Mensagem
);
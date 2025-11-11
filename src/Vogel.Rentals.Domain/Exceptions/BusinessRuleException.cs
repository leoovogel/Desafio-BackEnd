using System.Net;

namespace Vogel.Rentals.Domain.Exceptions;

public class BusinessRuleException(HttpStatusCode statusCode = HttpStatusCode.BadRequest, string message = "Dados inválidos")
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
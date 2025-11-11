using System.Net;

namespace Vogel.Rentals.Domain.Exceptions;

public class NotFoundException(string message) : BusinessRuleException(HttpStatusCode.NotFound, message)
{
    
}
using System.Net;
using System.Text.Json;
using Vogel.Rentals.Domain.Exceptions;

namespace Vogel.Rentals.Api.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex,
                "Business rule exception on {Path} | Message: {Message}",
                context.Request.Path,
                ex.Message);

            await WriteErrorResponseAsync(
                context,
                ex.StatusCode,
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex,
                "Error on {Path} | Message: {Message}",
                context.Request.Path,
                ex.Message);

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "Dados inválidos");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error on {Path}",
                context.Request.Path);

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = new { mensagem = message };

        var json = JsonSerializer.Serialize(payload);
        await context.Response.WriteAsync(json);
    }
}
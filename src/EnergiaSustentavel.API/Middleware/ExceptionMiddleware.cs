using System.Net;
using System.Text.Json;
using EnergiaSustentavel.API.Services;

namespace EnergiaSustentavel.API.Middleware;

/// <summary>
/// Middleware que captura exceções não tratadas e devolve uma resposta JSON
/// padronizada, com o status HTTP apropriado para cada tipo de erro.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarExcecaoAsync(context, ex);
        }
    }

    private async Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        var (status, titulo) = ex switch
        {
            RecursoNaoEncontradoException => (HttpStatusCode.NotFound, "Recurso não encontrado"),
            RegraNegocioException => (HttpStatusCode.BadRequest, "Requisição inválida"),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor")
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Erro não tratado: {Mensagem}", ex.Message);

        var resposta = new
        {
            status = (int)status,
            titulo,
            detalhe = ex.Message,
            // Em produção não expomos o stack trace.
            stackTrace = _env.IsDevelopment() ? ex.StackTrace : null,
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

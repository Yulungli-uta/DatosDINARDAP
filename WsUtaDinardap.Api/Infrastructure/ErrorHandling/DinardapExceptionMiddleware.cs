using WsUtaDinardap.Api.Application.Errors;
using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Infrastructure.ErrorHandling;

/// <summary>
/// Traduce DinardapException a ProblemDetails. Corre DESPUES de autenticacion/autorizacion
/// (UseAuthentication/UseAuthorization ya devolvieron 401/403 antes de llegar aca si aplicaba)
/// - un fallo de JWT/permiso NUNCA pasa por aqui ni se reporta como error DINARDAP_*.
/// </summary>
public sealed class DinardapExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DinardapExceptionMiddleware> _logger;

    public DinardapExceptionMiddleware(RequestDelegate next, ILogger<DinardapExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DinardapException ex)
        {
            var (status, title) = Map(ex.Code);
            _logger.LogWarning(ex, "DinardapException {Code} en {Path}", ex.Code, context.Request.Path);

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new
            {
                type = $"https://wsutadinardap/errors/{ex.Code}",
                title,
                status,
                detail = ex.Message,
                code = ex.Code.ToString(),
                traceId = context.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://wsutadinardap/errors/INTERNAL_ERROR",
                title = "Error interno",
                status = 500,
                detail = "Ocurrio un error inesperado.",
                code = DinardapErrorCode.InternalError.ToString(),
                traceId = context.TraceIdentifier
            });
        }
    }

    private static (int Status, string Title) Map(DinardapErrorCode code) => code switch
    {
        DinardapErrorCode.InvalidIdentification => (StatusCodes.Status400BadRequest, "Identificacion invalida"),
        DinardapErrorCode.NoData => (StatusCodes.Status404NotFound, "Sin datos"),
        DinardapErrorCode.DinardapTimeout => (StatusCodes.Status408RequestTimeout, "DINARDAP no respondio a tiempo"),
        DinardapErrorCode.DinardapUnavailable => (StatusCodes.Status502BadGateway, "DINARDAP no disponible"),
        DinardapErrorCode.DinardapUnauthorized => (StatusCodes.Status502BadGateway, "DINARDAP rechazo las credenciales"),
        DinardapErrorCode.DinardapInvalidResponse => (StatusCodes.Status502BadGateway, "Respuesta de DINARDAP invalida"),
        DinardapErrorCode.PackageNotAuthorized => (StatusCodes.Status403Forbidden, "Paquete no autorizado"),
        _ => (StatusCodes.Status500InternalServerError, "Error interno")
    };
}

public static class DinardapExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseDinardapExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<DinardapExceptionMiddleware>();
}

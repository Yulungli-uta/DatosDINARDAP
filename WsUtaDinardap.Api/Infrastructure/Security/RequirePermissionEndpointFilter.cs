using System.Security.Claims;

namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Mismo mecanismo que HrBackend.Infrastructure.Security.RequirePermissionEndpointFilter
/// (Minimal API, IEndpointFilter). Diferencia deliberada: aca "Authorization:ShadowMode"
/// por defecto es FALSE (enforce desde el dia 1), no true como en el rollout gradual de
/// HrBackend - DINARDAP expone datos personales sensibles (cedula, filiacion, titulos) y
/// no tiene un historial de roles ya asignados que necesite validarse en modo sombra primero.
/// </summary>
public sealed class RequirePermissionEndpointFilter : IEndpointFilter
{
    private readonly string _permissionCode;

    public RequirePermissionEndpointFilter(string permissionCode) => _permissionCode = permissionCode;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var user = http.User;

        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var permissionService = http.RequestServices.GetRequiredService<IUserActionPermissionService>();
        var config = http.RequestServices.GetRequiredService<IConfiguration>();
        var logger = http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RequirePermission");

        var shadowMode = bool.TryParse(config["Authorization:ShadowMode"], out var sm) && sm;

        var allowed = await permissionService.HasPermissionAsync(roles, _permissionCode, http.RequestAborted);

        if (!allowed)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("client_id")?.Value
                ?? "desconocido";

            if (shadowMode)
            {
                logger.LogWarning("[RequirePermission:SHADOW] {UserId} habria sido bloqueado en {Path} por falta de {Permission}",
                    userId, http.Request.Path, _permissionCode);
            }
            else
            {
                logger.LogWarning("[RequirePermission] {UserId} bloqueado en {Path} por falta de {Permission}",
                    userId, http.Request.Path, _permissionCode);
                return Results.Problem(
                    title: "Permiso insuficiente",
                    detail: $"Se requiere el permiso {_permissionCode}.",
                    statusCode: StatusCodes.Status403Forbidden);
            }
        }

        return await next(context);
    }
}

public static class RequirePermissionEndpointFilterExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permissionCode)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(new RequirePermissionEndpointFilter(permissionCode));
        return builder;
    }
}

using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Corre ANTES de UseAuthentication/UseAuthorization. Si la solicitud llega sin header
/// Authorization, viene de una IP en LegacyNetworkTrust:AllowedNetworks, y la opcion esta
/// habilitada, arma un ClaimsPrincipal sintetico con el rol configurado (por defecto
/// R_DINARDAP_LEGACY, el mismo que ya tiene los permisos DINARDAP.*.READ en WsSegu) - el
/// resto del pipeline (RequirePermissionEndpointFilter incluido) no distingue esto de un
/// JWT real, reutiliza exactamente la misma logica de autorizacion.
///
/// Un caller que SI trae Authorization sigue validandose por JWT normalmente - la confianza
/// por red es un camino adicional, no un reemplazo del JWT para quien ya lo tiene.
/// </summary>
public sealed class NetworkTrustMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NetworkTrustMiddleware> _logger;

    public NetworkTrustMiddleware(RequestDelegate next, ILogger<NetworkTrustMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext context, IOptions<NetworkTrustOptions> options)
    {
        var config = options.Value;

        if (config.Enabled
            && !context.Request.Headers.ContainsKey("Authorization")
            && IpNetworkMatcher.IsInAnyNetwork(context.Connection.RemoteIpAddress, config.AllowedNetworks))
        {
            var identity = new ClaimsIdentity(authenticationType: "NetworkTrust");
            identity.AddClaim(new Claim(ClaimTypes.Role, config.TrustedRole));
            identity.AddClaim(new Claim("auth_method", "network-trust"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"network-trust:{context.Connection.RemoteIpAddress}"));
            context.User = new ClaimsPrincipal(identity);

            _logger.LogInformation(
                "Solicitud confiada por red desde {RemoteIp} en {Path} - rol asignado: {Role}",
                context.Connection.RemoteIpAddress, context.Request.Path, config.TrustedRole);
        }

        return _next(context);
    }
}

public static class NetworkTrustMiddlewareExtensions
{
    public static IApplicationBuilder UseLegacyNetworkTrust(this IApplicationBuilder app) =>
        app.UseMiddleware<NetworkTrustMiddleware>();
}

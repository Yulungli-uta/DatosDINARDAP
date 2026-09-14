using WsUtaDinardap.Api.Application;
using WsUtaDinardap.Api.Infrastructure.Security;

namespace WsUtaDinardap.Api.Endpoints;

/// <summary>
/// Superficie publica inicial: un endpoint fijo por paquete (no un /{package}/{id} generico
/// que permita consultar cualquier paquete arbitrario). La logica generica por paquete vive
/// internamente en DinardapService/IDinardapPackageHandler.
/// </summary>
public static class DinardapEndpoints
{
    public static void MapDinardapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/dinardap")
            .RequireAuthorization()
            .WithTags("Dinardap");

        group.MapGet("/registro-civil/{identificacion}", async (string identificacion, IDinardapService service, CancellationToken ct) =>
            {
                var data = await service.ConsultarRegistroCivilAsync(identificacion, ct);
                return Results.Ok(data);
            })
            .RequirePermission(DinardapPermissions.RegistroCivilConsultar);

        group.MapGet("/titulos/{identificacion}", async (string identificacion, IDinardapService service, CancellationToken ct) =>
            {
                var data = await service.ConsultarTitulosAsync(identificacion, ct);
                return Results.Ok(data);
            })
            .RequirePermission(DinardapPermissions.TitulosConsultar);

        group.MapGet("/tce/{identificacion}", async (string identificacion, IDinardapService service, CancellationToken ct) =>
            {
                var data = await service.ConsultarTceAsync(identificacion, ct);
                return Results.Ok(data);
            })
            .RequirePermission(DinardapPermissions.TceConsultar);
    }
}

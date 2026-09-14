namespace WsUtaDinardap.Api.Application.Interfaces;

/// <summary>
/// Forma neutral (no depende de tipos generados por WCF) de la respuesta getFichaGeneral
/// de DINARDAP. Infrastructure mapea el proxy SOAP a esto; los PackageHandler de
/// Application solo conocen esta forma, nunca System.ServiceModel.
/// </summary>
public sealed record FichaGeneralDto(
    string? CodigoPaquete,
    string? Mensaje,
    IReadOnlyList<InstitucionDto> Instituciones);

public sealed record InstitucionDto(
    string? Nombre,
    string? Mensaje,
    IReadOnlyList<RegistroDto> DatosPrincipales,
    IReadOnlyList<ItemDto> Detalle);

public sealed record RegistroDto(string? Campo, string? Codigo, string? Valor);

public sealed record ItemDto(string? Nombre, IReadOnlyList<RegistroDto> Registros);

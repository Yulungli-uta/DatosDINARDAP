using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Interfaces;

/// <summary>
/// Unico componente que sabe hablar SOAP con DINARDAP (endpoint, credenciales, binding,
/// timeout). No conoce WsSegu, JWT, roles ni permisos - eso es responsabilidad de otra capa.
/// Lanza DinardapException con el DinardapErrorCode correspondiente ante cualquier falla;
/// nunca deja escapar una excepcion de WCF/SOAP cruda hacia Application.
/// </summary>
public interface IDinardapGateway
{
    Task<FichaGeneralDto> ConsultarAsync(string numeroIdentificacion, DinardapPackage paquete, CancellationToken ct);
}

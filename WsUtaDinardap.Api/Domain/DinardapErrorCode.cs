namespace WsUtaDinardap.Api.Domain;

/// <summary>
/// Errores tipados de la API. Separado a proposito de los errores de autenticacion/autorizacion
/// UTA (WsSegu): un 401 por JWT invalido nunca se traduce a DINARDAP_UNAVAILABLE ni similar.
/// </summary>
public enum DinardapErrorCode
{
    InvalidIdentification,
    NoData,
    DinardapTimeout,
    DinardapUnavailable,
    DinardapUnauthorized,
    DinardapInvalidResponse,
    PackageNotAuthorized,
    InternalError
}

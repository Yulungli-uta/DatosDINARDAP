using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Errors;

/// <summary>
/// Unica excepcion que cruza de Infrastructure/Application hacia los endpoints.
/// El middleware de errores la mapea a ProblemDetails segun DinardapErrorCode.
/// </summary>
public sealed class DinardapException : Exception
{
    public DinardapErrorCode Code { get; }

    public DinardapException(DinardapErrorCode code, string message, Exception? inner = null)
        : base(message, inner)
    {
        Code = code;
    }
}

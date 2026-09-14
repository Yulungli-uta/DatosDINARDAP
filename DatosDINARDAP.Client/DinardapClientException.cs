using DatosDINARDAP.Client.Models;

namespace DatosDINARDAP.Client;

/// <summary>Excepcion lanzada por IDinardapClient ante cualquier respuesta de error de WsUtaDinardap.Api.</summary>
public sealed class DinardapClientException : Exception
{
    public int StatusCode { get; }
    public string? Code { get; }
    public DinardapProblemDetails? ProblemDetails { get; }

    public DinardapClientException(int statusCode, string? code, string message, DinardapProblemDetails? problemDetails = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        ProblemDetails = problemDetails;
    }
}

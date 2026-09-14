namespace WsUtaDinardap.Api.Infrastructure.Dinardap;

/// <summary>
/// Nunca poner Username/Password reales en appsettings.json versionado - solo variables de
/// entorno (Dinardap__Username / Dinardap__Password) o un Secret Manager en produccion.
/// </summary>
public sealed class DinardapOptions
{
    public const string SectionName = "Dinardap";

    public string Endpoint { get; set; } = "https://interoperabilidad.dinardap.gob.ec:7979/interoperador";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

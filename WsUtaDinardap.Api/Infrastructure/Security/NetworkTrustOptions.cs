namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Confianza por red para consumidores que no pueden llevar credencial propia (ej. la DLL
/// legacy, referenciada en muchos proyectos incluyendo maquinas de desarrollador - repartir
/// un secreto ahi es peor que no tener ninguno). Deshabilitada por defecto: hay que activarla
/// y listar explicitamente las redes internas reales antes de que tenga efecto.
/// </summary>
public sealed class NetworkTrustOptions
{
    public const string SectionName = "LegacyNetworkTrust";

    public bool Enabled { get; set; } = false;

    /// <summary>IPs exactas o rangos CIDR (ej. "10.102.12.0/24") de la red interna institucional.</summary>
    public List<string> AllowedNetworks { get; set; } = new();

    /// <summary>Rol sintetico asignado a las solicitudes confiadas por red - debe tener ya los permisos DINARDAP.*.READ en WsSegu.</summary>
    public string TrustedRole { get; set; } = "R_DINARDAP_LEGACY";
}

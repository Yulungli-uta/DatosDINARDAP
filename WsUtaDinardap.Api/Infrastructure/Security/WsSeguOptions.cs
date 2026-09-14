namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Configuracion de integracion con WsSegu (RepositoryUta). Mismos nombres de clave que
/// HrBackend (AuthService:*) para no introducir una convencion nueva en el ecosistema.
/// </summary>
public sealed class WsSeguOptions
{
    public const string SectionName = "AuthService";

    public string Url { get; set; } = "http://localhost:5010";
    public string JwksUrl { get; set; } = "/.well-known/jwks.json";
    public string EffectivePermissionsUrl { get; set; } = "/api/role-permissions/effective";
    public string Issuer { get; set; } = "WsSeguUta.AuthSystem.API";
    public string Audience { get; set; } = "WsSeguUta.AuthSystem.API";
    public int JwksCacheHours { get; set; } = 24;
    public int PermissionsCacheMinutes { get; set; } = 5;
}

namespace DatosDINARDAP.Client;

/// <summary>
/// El consumidor decide como obtiene el Bearer token (reenviar el JWT del usuario actual,
/// o pedir uno propio via Client Credentials de WsSegu para procesos sin usuario interactivo).
/// DatosDINARDAP.Client no sabe de login ni de WsSegu, solo pide "dame el token vigente".
/// </summary>
public interface IDinardapTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken ct);
}

namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Codigos de permiso DINARDAP, convencion "MODULE.ACTION" identica a auth.tbl_Permissions
/// en WsSegu (ver GET /api/role-permissions/effective). Centralizados aqui para no repetir
/// literales "DINARDAP..." en cada endpoint.
/// </summary>
public static class DinardapPermissions
{
    // Accion "READ" (no "CONSULTAR") a proposito: auth.tbl_Permissions.Action tiene un CHECK
    // constraint con una lista fija de verbos en ingles (READ/CREATE/UPDATE/...) que ya usa
    // el resto del catalogo institucional - reutilizarla evita otra ampliacion del constraint.
    public const string RegistroCivilConsultar = "DINARDAP.REGISTRO_CIVIL.READ";
    public const string TceConsultar = "DINARDAP.TCE.READ";
    public const string TitulosConsultar = "DINARDAP.TITULOS.READ";

    /// <summary>Bypass institucional: cualquier permiso efectivo que incluya este codigo pasa cualquier chequeo.</summary>
    public const string AdminAccess = "ADMIN.ACCESS";
}

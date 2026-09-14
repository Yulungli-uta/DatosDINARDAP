namespace WsUtaDinardap.Api.Domain;

public sealed record RegistroCivilData(
    string? Codigo,
    string? NombreCompleto,
    string? Nombres,
    string? Apellido1,
    string? Apellido2,
    string? Genero,
    string? CondicionCiudadano,
    DateTime? FechaNacimiento,
    string? LugarNacimiento,
    string? LugarNacimientoProvincia,
    string? LugarNacimientoCiudad,
    string? LugarNacimientoParroquia,
    string? Nacionalidad,
    string? EstadoCivil,
    string? Conyuge,
    string? NombrePadre,
    string? NombreMadre);

public sealed record TceData(
    string? NumeroCertificado,
    DateTime? FechaSufragio,
    bool? Sufrago);

public sealed record TituloData(
    string? NivelNombre,
    int Nivel,
    DateTime? FechaGrado,
    DateTime? FechaRegistro,
    string? InstitucionEducacionSuperior,
    string? NombreTitulo,
    string? NumeroRegistro,
    string? Tipo,
    string? TipoExtranjeroColegio);

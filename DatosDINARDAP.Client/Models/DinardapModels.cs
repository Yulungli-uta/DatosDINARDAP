namespace DatosDINARDAP.Client.Models;

// Mismos campos que WsUtaDinardap.Api.Domain.DinardapModels - deliberadamente NO se comparte
// el ensamblado Domain entre API y Client (estan a los dos lados de un limite HTTP), solo
// el contrato/forma de los datos.

public sealed record RegistroCivilDto(
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

public sealed record TceDto(
    string? NumeroCertificado,
    DateTime? FechaSufragio,
    bool? Sufrago);

public sealed record TituloDto(
    string? NivelNombre,
    int Nivel,
    DateTime? FechaGrado,
    DateTime? FechaRegistro,
    string? InstitucionEducacionSuperior,
    string? NombreTitulo,
    string? NumeroRegistro,
    string? Tipo,
    string? TipoExtranjeroColegio);

/// <summary>Forma del error tipado que devuelve WsUtaDinardap.Api (DinardapExceptionMiddleware).</summary>
public sealed record DinardapProblemDetails(
    string? Type,
    string? Title,
    int Status,
    string? Detail,
    string? Code,
    string? TraceId);

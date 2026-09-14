using System.Globalization;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Handlers;

/// <summary>
/// Interpreta el paquete 113 (Registro Civil). Codigos de campo tal como los usa
/// hoy DatosDINARDAP.Datos.DAL.RegistroCivilDAL (1=cedula, 2=nombre, 3=genero, ...).
/// </summary>
public sealed class RegistroCivilPackageHandler : IDinardapPackageHandler<RegistroCivilData>
{
    public DinardapPackage Package => DinardapPackage.RegistroCivil;

    public RegistroCivilData Parse(FichaGeneralDto ficha)
    {
        string? codigo = null, nombreCompleto = null, nombres = null, apellido1 = null, apellido2 = null;
        string? genero = null, condicionCiudadano = null, lugarNacimiento = null;
        string? lugarNacimientoProvincia = null, lugarNacimientoCiudad = null, lugarNacimientoParroquia = null;
        string? nacionalidad = null, estadoCivil = null, conyuge = null, nombrePadre = null, nombreMadre = null;
        DateTime? fechaNacimiento = null;

        foreach (var institucion in ficha.Instituciones)
        {
            foreach (var registro in institucion.DatosPrincipales)
            {
                switch (registro.Codigo)
                {
                    case "1": // Cedula
                        codigo = registro.Valor;
                        break;
                    case "2": // nombre
                        var (completo, ap1, ap2, nom) = SepararApellidos(registro.Valor);
                        nombreCompleto = completo;
                        apellido1 = ap1;
                        apellido2 = ap2;
                        nombres = nom;
                        break;
                    case "3": // genero
                        genero = registro.Valor;
                        break;
                    case "4": // Condicion ciudadano
                        condicionCiudadano = registro.Valor;
                        break;
                    case "5": // fecha nacimiento dd/mm/yyyy
                        fechaNacimiento = ParseFecha(registro.Valor, "dd/MM/yyyy");
                        break;
                    case "6": // lugar nacimiento
                        lugarNacimiento = registro.Valor;
                        var partes = lugarNacimiento?.Split('/') ?? Array.Empty<string>();
                        if (partes.Length > 0) lugarNacimientoProvincia = partes[0];
                        if (partes.Length > 1) lugarNacimientoCiudad = partes[1];
                        if (partes.Length > 2) lugarNacimientoParroquia = partes[2];
                        break;
                    case "7": // nacionalidad
                        nacionalidad = registro.Valor;
                        break;
                    case "8": // estado civil
                        estadoCivil = registro.Valor;
                        break;
                    case "10": // conyuge
                        conyuge = registro.Valor;
                        break;
                    case "11": // nombre padre
                        nombrePadre = registro.Valor;
                        break;
                    case "13": // nombre madre
                        nombreMadre = registro.Valor;
                        break;
                }
            }
        }

        return new RegistroCivilData(
            codigo, nombreCompleto, nombres, apellido1, apellido2, genero, condicionCiudadano,
            fechaNacimiento, lugarNacimiento, lugarNacimientoProvincia, lugarNacimientoCiudad,
            lugarNacimientoParroquia, nacionalidad, estadoCivil, conyuge, nombrePadre, nombreMadre);
    }

    /// <summary>
    /// Puerto exacto de SepararApellidos (DatosDINARDAP.Datos.DAL.RegistroCivilDAL) para no
    /// alterar el criterio de separacion apellido1/apellido2/nombres ya validado en produccion.
    /// </summary>
    private static (string Completo, string? Apellido1, string? Apellido2, string? Nombres) SepararApellidos(string? nombreCompleto)
    {
        if (string.IsNullOrEmpty(nombreCompleto))
            return (string.Empty, null, null, null);

        var partesOriginales = nombreCompleto.Split(' ');
        var nombreSeparado = string.Empty;
        foreach (var parte in partesOriginales)
        {
            if (parte.Length == 0) continue;

            switch (parte.ToLowerInvariant())
            {
                case "de":
                case "del":
                case "la":
                case "las":
                case "los":
                case "san":
                    nombreSeparado += parte + " ";
                    break;
                default:
                    nombreSeparado += parte + "@";
                    break;
            }
        }

        if (nombreSeparado.Length > 0 && nombreSeparado[^1] == '@')
            nombreSeparado = nombreSeparado[..^1];

        var completo = nombreSeparado.Replace('@', ' ');
        var partes = nombreSeparado.Split('@');

        string? apellido1 = null, apellido2 = null, nombres = null;
        if (partes.Length == 1)
        {
            apellido1 = partes[0];
        }
        else if (partes.Length == 2)
        {
            apellido1 = partes[0];
            nombres = partes[1];
        }
        else if (partes.Length >= 3)
        {
            apellido1 = partes[0];
            apellido2 = partes[1];
            for (var i = 2; i < partes.Length; i++)
                nombres += (string.IsNullOrEmpty(nombres) ? "" : " ") + partes[i];
        }

        return (completo, apellido1, apellido2, nombres);
    }

    private static DateTime? ParseFecha(string? valor, string formato) =>
        DateTime.TryParseExact(valor, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha)
            ? fecha
            : null;
}

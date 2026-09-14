using System.Globalization;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Handlers;

/// <summary>
/// Interpreta el paquete 115 (Titulos/SENESCYT). Codigos de campo y clasificacion de nivel
/// tal como los usa hoy DatosDINARDAP.Datos.DAL.TitulosDAL.
/// </summary>
public sealed class TitulosPackageHandler : IDinardapPackageHandler<IReadOnlyList<TituloData>>
{
    public DinardapPackage Package => DinardapPackage.Titulos;

    public IReadOnlyList<TituloData> Parse(FichaGeneralDto ficha)
    {
        var lista = new List<TituloData>();

        foreach (var institucion in ficha.Instituciones)
        {
            foreach (var item in institucion.Detalle)
            {
                var nivel = ClasificarNivel(item.Nombre);

                string? fechaGradoRaw = null, fechaRegistroRaw = null, institucionEs = null,
                    nombreTitulo = null, numeroRegistro = null, tipo = null, tipoExtranjeroColegio = null;

                foreach (var registro in item.Registros)
                {
                    switch (registro.Codigo)
                    {
                        case "51": fechaGradoRaw = registro.Valor; break;
                        case "52": fechaRegistroRaw = registro.Valor; break;
                        case "53": institucionEs = registro.Valor; break;
                        case "54": nombreTitulo = registro.Valor; break;
                        case "55": numeroRegistro = registro.Valor; break;
                        case "56": tipo = registro.Valor; break;
                        case "57": tipoExtranjeroColegio = registro.Valor; break;
                    }
                }

                lista.Add(new TituloData(
                    item.Nombre,
                    nivel,
                    ParseFechaYmd(fechaGradoRaw),
                    ParseFechaYmd(fechaRegistroRaw),
                    institucionEs,
                    nombreTitulo,
                    numeroRegistro,
                    tipo,
                    tipoExtranjeroColegio));
            }
        }

        return lista;
    }

    private static int ClasificarNivel(string? nombreNivel) => nombreNivel switch
    {
        "Títulos de Tercer Nivel" or "Tercer Nivel o Pregrado" => 3,
        "Títulos Cuarto Nivel" or "Cuarto Nivel o Posgrado" or "CUARTO_NIVEL" => 4,
        _ => 2
    };

    /// <summary>
    /// yyyy/mm/dd, largo minimo 8 caracteres - igual criterio que TitulosDAL actual.
    /// A diferencia del legacy (que devuelve 1900-01-01 como centinela de "sin fecha"),
    /// aqui null significa honestamente "sin fecha"; el traductor legacy hace el
    /// downgrade a 1900-01-01 solo para el consumidor de la DLL vieja.
    /// </summary>
    private static DateTime? ParseFechaYmd(string? valor)
    {
        if (string.IsNullOrEmpty(valor) || valor.Length < 8) return null;
        return DateTime.TryParseExact(valor, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var f)
            ? f
            : null;
    }
}

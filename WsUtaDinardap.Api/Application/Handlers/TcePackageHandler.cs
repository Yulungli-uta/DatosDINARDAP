using System.Globalization;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Handlers;

/// <summary>
/// Interpreta el paquete 114 (TCE). Codigos de campo tal como los usa hoy
/// DatosDINARDAP.Datos.DAL.TCEDAL (44=numero certificado, 45=fecha sufragio, 46=sufrago SI/NO).
/// </summary>
public sealed class TcePackageHandler : IDinardapPackageHandler<TceData>
{
    public DinardapPackage Package => DinardapPackage.Tce;

    public TceData Parse(FichaGeneralDto ficha)
    {
        string? numeroCertificado = null;
        DateTime? fechaSufragio = null;
        bool? sufrago = null;

        foreach (var institucion in ficha.Instituciones)
        {
            foreach (var registro in institucion.DatosPrincipales)
            {
                switch (registro.Codigo)
                {
                    case "44": // Numero de certificado
                        numeroCertificado = registro.Valor;
                        break;
                    case "45": // fecha de votacion yyyy/MM/dd
                        fechaSufragio = DateTime.TryParseExact(registro.Valor, "yyyy/MM/dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) ? f : null;
                        break;
                    case "46": // sufrago SI/NO
                        sufrago = string.Equals(registro.Valor, "SI", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }
        }

        return new TceData(numeroCertificado, fechaSufragio, sufrago);
    }
}

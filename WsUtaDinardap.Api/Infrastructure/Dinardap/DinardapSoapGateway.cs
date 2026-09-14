using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Extensions.Options;
using WsUtaDinardap.Api.Application.Errors;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;
using WsUtaDinardap.Api.Infrastructure.Dinardap.Proxy;

namespace WsUtaDinardap.Api.Infrastructure.Dinardap;

/// <summary>
/// Unico componente que conoce endpoint/credenciales/binding/timeout de DINARDAP.
/// No conoce WsSegu, JWT, roles ni permisos. Traduce cualquier falla de la llamada SOAP a
/// un DinardapException con codigo tipado; nunca deja escapar una excepcion de WCF cruda.
/// </summary>
public sealed class DinardapSoapGateway : IDinardapGateway
{
    private readonly DinardapOptions _options;
    private readonly ILogger<DinardapSoapGateway> _logger;

    public DinardapSoapGateway(IOptions<DinardapOptions> options, ILogger<DinardapSoapGateway> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FichaGeneralDto> ConsultarAsync(string numeroIdentificacion, DinardapPackage paquete, CancellationToken ct)
    {
        var binding = new BasicHttpBinding
        {
            Security =
            {
                Mode = BasicHttpSecurityMode.Transport,
                Transport = { ClientCredentialType = HttpClientCredentialType.Basic }
            },
            SendTimeout = TimeSpan.FromSeconds(_options.TimeoutSeconds),
            ReceiveTimeout = TimeSpan.FromSeconds(_options.TimeoutSeconds),
            OpenTimeout = TimeSpan.FromSeconds(_options.TimeoutSeconds)
        };

        var client = new InteroperadorClient(binding, new EndpointAddress(_options.Endpoint));
        client.ClientCredentials.UserName.UserName = _options.Username;
        client.ClientCredentials.UserName.Password = _options.Password;

        try
        {
            var codigoPaquete = ((int)paquete).ToString();
            var ficha = await client.GetFichaGeneralAsync(numeroIdentificacion, codigoPaquete);

            if (ficha is null)
                throw new DinardapException(DinardapErrorCode.DinardapInvalidResponse,
                    "DINARDAP respondio sin cuerpo (fichaGeneral nulo).");

            await CloseAsync(client);
            return Mapear(ficha);
        }
        catch (DinardapException)
        {
            Abort(client);
            throw;
        }
        catch (TimeoutException ex)
        {
            Abort(client);
            _logger.LogWarning(ex, "Timeout consultando DINARDAP paquete {Paquete}", paquete);
            throw new DinardapException(DinardapErrorCode.DinardapTimeout, "DINARDAP no respondio a tiempo.", ex);
        }
        catch (MessageSecurityException ex)
        {
            Abort(client);
            _logger.LogWarning(ex, "Credenciales DINARDAP rechazadas para paquete {Paquete}", paquete);
            throw new DinardapException(DinardapErrorCode.DinardapUnauthorized, "DINARDAP rechazo las credenciales configuradas.", ex);
        }
        catch (EndpointNotFoundException ex)
        {
            Abort(client);
            _logger.LogError(ex, "DINARDAP no disponible (endpoint no encontrado) para paquete {Paquete}", paquete);
            throw new DinardapException(DinardapErrorCode.DinardapUnavailable, "DINARDAP no esta disponible.", ex);
        }
        catch (CommunicationException ex)
        {
            Abort(client);
            _logger.LogError(ex, "Error de comunicacion con DINARDAP para paquete {Paquete}", paquete);
            throw new DinardapException(DinardapErrorCode.DinardapUnavailable, "No se pudo comunicar con DINARDAP.", ex);
        }
        catch (Exception ex)
        {
            Abort(client);
            _logger.LogError(ex, "Error inesperado consultando DINARDAP paquete {Paquete}", paquete);
            throw new DinardapException(DinardapErrorCode.InternalError, "Error inesperado consultando DINARDAP.", ex);
        }
    }

    private static FichaGeneralDto Mapear(fichaGeneral ficha) => new(
        ficha.codigoPaquete,
        ficha.mensaje,
        (ficha.instituciones ?? Array.Empty<institucion>())
            .Select(i => new InstitucionDto(
                i.nombre,
                i.mensaje,
                (i.datosPrincipales ?? Array.Empty<registro>()).Select(MapearRegistro).ToList(),
                (i.detalle ?? Array.Empty<item>()).Select(MapearItem).ToList()))
            .ToList());

    private static ItemDto MapearItem(item item) => new(
        item.nombre,
        (item.registros ?? Array.Empty<registro>()).Select(MapearRegistro).ToList());

    private static RegistroDto MapearRegistro(registro r) => new(r.campo, r.codigo, r.valor);

    private static async Task CloseAsync(InteroperadorClient client)
    {
        try
        {
            if (client.State == CommunicationState.Opened)
                await Task.Run(client.Close);
            else
                client.Abort();
        }
        catch
        {
            client.Abort();
        }
    }

    private static void Abort(InteroperadorClient client)
    {
        try { client.Abort(); } catch { /* ya se esta cerrando por la falla original */ }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DatosDINARDAP.Datos.Infrastructure
{
    /// <summary>
    /// Unico punto de la DLL legacy que sabe hablar HTTP con WsUtaDinardap.Api. Sustituye a
    /// la construccion directa de InteroperadorClient que hacian antes RegistroCivilDAL/
    /// TCEDAL/TitulosDAL - ya no hay credenciales DINARDAP en esta DLL, ni SOAP.
    ///
    /// Sin token ni secreto propio: WsUtaDinardap.Api confia esta llamada por la IP de
    /// origen del servidor (LegacyNetworkTrust, configurado del lado de la API) - repartir
    /// un secreto a cada proyecto/maquina que referencia esta DLL era peor que no tener
    /// ninguno. Si la IP de origen no esta en la lista de confianza del servidor, la API
    /// simplemente responde 401 - se ve igual que cualquier otra falla desde el punto de
    /// vista de esta DLL (catch generico en cada DAL).
    /// </summary>
    internal static class DinardapApiClient
    {
        private static readonly HttpClient Http = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(WsUtaDinardapApiOptions.BaseUrl) };
            client.Timeout = TimeSpan.FromSeconds(WsUtaDinardapApiOptions.TimeoutSeconds);
            return client;
        }

        public static async Task<T> GetAsync<T>(string relativePath)
        {
            HttpResponseMessage response;
            try
            {
                response = await Http.GetAsync(relativePath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new DinardapApiCallException("No se pudo comunicar con WsUtaDinardap.Api.", ex);
            }

            using (response)
            {
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(body);
                    }
                    catch (Exception ex)
                    {
                        throw new DinardapApiCallException("Respuesta de WsUtaDinardap.Api con formato inesperado.", ex);
                    }
                }

                DinardapProblemDetailsDto problem = null;
                try
                {
                    problem = JsonConvert.DeserializeObject<DinardapProblemDetailsDto>(body);
                }
                catch
                {
                    // el cuerpo de error no vino en el formato esperado; se reporta igual con el status code
                }

                throw new DinardapApiCallException(
                    problem != null && problem.Detail != null
                        ? problem.Detail
                        : "WsUtaDinardap.Api respondio " + (int)response.StatusCode,
                    null);
            }
        }
    }

    /// <summary>Envuelve cualquier falla de red/HTTP/deserializacion al hablar con WsUtaDinardap.Api.</summary>
    internal sealed class DinardapApiCallException : Exception
    {
        public DinardapApiCallException(string message, Exception inner) : base(message, inner) { }
    }
}

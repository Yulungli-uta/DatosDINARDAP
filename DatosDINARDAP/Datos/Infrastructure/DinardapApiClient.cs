using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DatosDINARDAP.Datos.Infrastructure
{
    /// <summary>
    /// Unico punto de la DLL legacy que sabe hablar HTTP con WsUtaDinardap.Api y obtener un
    /// token tecnico de WsSegu (Client Credentials). Sustituye a la construccion directa de
    /// InteroperadorClient que hacian antes RegistroCivilDAL/TCEDAL/TitulosDAL - ya no hay
    /// credenciales DINARDAP en esta DLL, ni SOAP.
    ///
    /// Cliente y token se comparten (estaticos) entre las 3 DAL para no pedir un token nuevo
    /// en cada consulta - el token de aplicacion de WsSegu vive 60 minutos.
    /// </summary>
    internal static class DinardapApiClient
    {
        private static readonly HttpClient Http = CreateHttpClient();
        private static readonly object TokenLock = new object();
        private static string _cachedToken;
        private static DateTime _tokenExpiresAtUtc = DateTime.MinValue;

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(WsUtaDinardapApiOptions.BaseUrl) };
            client.Timeout = TimeSpan.FromSeconds(WsUtaDinardapApiOptions.TimeoutSeconds);
            return client;
        }

        public static async Task<T> GetAsync<T>(string relativePath)
        {
            var token = await GetTokenAsync().ConfigureAwait(false);

            using (var request = new HttpRequestMessage(HttpMethod.Get, relativePath))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;
                try
                {
                    response = await Http.SendAsync(request).ConfigureAwait(false);
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

        private static async Task<string> GetTokenAsync()
        {
            lock (TokenLock)
            {
                if (_cachedToken != null && DateTime.UtcNow < _tokenExpiresAtUtc)
                    return _cachedToken;
            }

            var payload = new
            {
                clientId = WsUtaDinardapApiOptions.ClientId,
                clientSecret = WsUtaDinardapApiOptions.ClientSecret
            };

            using (var tokenClient = new HttpClient { BaseAddress = new Uri(WsUtaDinardapApiOptions.AuthServiceUrl) })
            {
                tokenClient.Timeout = TimeSpan.FromSeconds(WsUtaDinardapApiOptions.TimeoutSeconds);

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                try
                {
                    response = await tokenClient.PostAsync("/api/app-auth/token", content).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new DinardapApiCallException("No se pudo obtener token tecnico de WsSegu.", ex);
                }

                using (response)
                {
                    if (!response.IsSuccessStatusCode)
                        throw new DinardapApiCallException("WsSegu rechazo las credenciales tecnicas de la aplicacion.", null);

                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    dynamic parsed = JsonConvert.DeserializeObject(body);
                    string accessToken = parsed?.data?.accessToken;

                    if (string.IsNullOrEmpty(accessToken))
                        throw new DinardapApiCallException("WsSegu no devolvio un token de aplicacion valido.", null);

                    lock (TokenLock)
                    {
                        _cachedToken = accessToken;
                        // 55 min de margen sobre la vida real de 60 min del token de aplicacion en WsSegu.
                        _tokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(55);
                    }

                    return accessToken;
                }
            }
        }
    }

    /// <summary>Envuelve cualquier falla de red/HTTP/deserializacion al hablar con WsUtaDinardap.Api o WsSegu.</summary>
    internal sealed class DinardapApiCallException : Exception
    {
        public DinardapApiCallException(string message, Exception inner) : base(message, inner) { }
    }
}

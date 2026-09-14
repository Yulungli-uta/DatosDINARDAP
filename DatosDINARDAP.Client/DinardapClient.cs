using System.Net.Http.Json;
using System.Text.Json;
using DatosDINARDAP.Client.Models;

namespace DatosDINARDAP.Client;

/// <summary>
/// Cliente HTTP para WsUtaDinardap.Api, pensado para HttpClientFactory (AddDinardapClient).
/// No conoce SOAP ni credenciales DINARDAP - eso vive exclusivamente en WsUtaDinardap.Api.
/// </summary>
public sealed class DinardapClient : IDinardapClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public DinardapClient(HttpClient http) => _http = http;

    public Task<RegistroCivilDto> ObtenerRegistroCivilAsync(string identificacion, CancellationToken ct = default) =>
        GetAsync<RegistroCivilDto>($"/api/v1/dinardap/registro-civil/{Uri.EscapeDataString(identificacion)}", ct);

    public Task<TceDto> ObtenerTceAsync(string identificacion, CancellationToken ct = default) =>
        GetAsync<TceDto>($"/api/v1/dinardap/tce/{Uri.EscapeDataString(identificacion)}", ct);

    public async Task<IReadOnlyList<TituloDto>> ObtenerTitulosAsync(string identificacion, CancellationToken ct = default) =>
        await GetAsync<List<TituloDto>>($"/api/v1/dinardap/titulos/{Uri.EscapeDataString(identificacion)}", ct) ?? [];

    private async Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct)
    {
        using var response = await _http.GetAsync(relativeUrl, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
            return result ?? throw new DinardapClientException((int)response.StatusCode, "EMPTY_RESPONSE", "WsUtaDinardap.Api devolvio un cuerpo vacio.");
        }

        DinardapProblemDetails? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<DinardapProblemDetails>(JsonOptions, ct);
        }
        catch
        {
            // el cuerpo de error no vino en el formato esperado; se reporta igual con lo que se tenga
        }

        throw new DinardapClientException(
            (int)response.StatusCode,
            problem?.Code,
            problem?.Detail ?? $"WsUtaDinardap.Api respondio {(int)response.StatusCode}.",
            problem);
    }
}

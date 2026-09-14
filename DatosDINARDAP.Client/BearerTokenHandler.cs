using System.Net.Http.Headers;

namespace DatosDINARDAP.Client;

internal sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly IDinardapTokenProvider _tokenProvider;

    public BearerTokenHandler(IDinardapTokenProvider tokenProvider) => _tokenProvider = tokenProvider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _tokenProvider.GetAccessTokenAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}

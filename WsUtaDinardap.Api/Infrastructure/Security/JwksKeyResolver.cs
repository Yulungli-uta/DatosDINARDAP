using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>
/// Resuelve las claves de firma RS256 de WsSegu desde /.well-known/jwks.json, cacheadas.
/// Mismo patron que HrBackend.Infrastructure.Services.LocalJwtValidationService (ya en
/// produccion en ese repo) - aqui expuesto como IssuerSigningKeyResolver estandar de
/// Microsoft.AspNetCore.Authentication.JwtBearer en vez de un middleware manual, por ser
/// un proyecto nuevo sin el historial que obligo a HrBackend a su propio middleware.
/// </summary>
public sealed class JwksKeyResolver
{
    private const string CacheKey = "wssegu_jwks";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly WsSeguOptions _options;
    private readonly ILogger<JwksKeyResolver> _logger;

    public JwksKeyResolver(IHttpClientFactory httpClientFactory, IMemoryCache cache,
        IOptions<WsSeguOptions> options, ILogger<JwksKeyResolver> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public IEnumerable<SecurityKey> ResolveSigningKeys(string token, SecurityToken securityToken, string kid, TokenValidationParameters parameters)
    {
        // El delegado de IdentityModel es sincrono; el JWKS ya esta cacheado casi siempre
        // (24h), asi que bloquear aqui en el hit de cache no bloquea IO real.
        return GetKeysAsync().GetAwaiter().GetResult();
    }

    private async Task<IReadOnlyList<SecurityKey>> GetKeysAsync()
    {
        if (_cache.TryGetValue<IReadOnlyList<SecurityKey>>(CacheKey, out var cached) && cached is not null)
            return cached;

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        var jwksUrl = $"{_options.Url.TrimEnd('/')}/{_options.JwksUrl.TrimStart('/')}";
        var json = await client.GetStringAsync(jwksUrl);
        var jwks = new JsonWebKeySet(json);
        var keys = jwks.GetSigningKeys().ToList();

        if (keys.Count == 0)
        {
            _logger.LogError("JWKS en {Url} no contiene claves de firma utilizables", jwksUrl);
            return Array.Empty<SecurityKey>();
        }

        _cache.Set(CacheKey, (IReadOnlyList<SecurityKey>)keys, TimeSpan.FromHours(_options.JwksCacheHours));
        _logger.LogInformation("Claves JWKS de WsSegu obtenidas y cacheadas por {Hours}h desde {Url}", _options.JwksCacheHours, jwksUrl);
        return keys;
    }
}

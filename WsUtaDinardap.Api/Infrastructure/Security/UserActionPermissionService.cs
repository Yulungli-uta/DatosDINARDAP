using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WsUtaDinardap.Api.Infrastructure.Security;

public interface IUserActionPermissionService
{
    Task<bool> HasPermissionAsync(IEnumerable<string> roles, string permissionCode, CancellationToken ct);
}

/// <summary>
/// Mismo mecanismo que HrBackend.Infrastructure.Services.UserActionPermissionService: consulta
/// el endpoint publico de efectivos de WsSegu (GET /api/role-permissions/effective?roles=...),
/// cachea por conjunto de roles, y respeta el bypass ADMIN.ACCESS ya vigente en el ecosistema.
/// No se reimplementa el modelo de permisos - se reutiliza el catalogo/endpoint existente.
/// </summary>
public sealed class UserActionPermissionService : IUserActionPermissionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly WsSeguOptions _options;
    private readonly ILogger<UserActionPermissionService> _logger;

    public UserActionPermissionService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
        IOptions<WsSeguOptions> options, ILogger<UserActionPermissionService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> HasPermissionAsync(IEnumerable<string> roles, string permissionCode, CancellationToken ct)
    {
        var roleList = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().OrderBy(r => r).ToList();
        if (roleList.Count == 0) return false;

        var effective = await GetEffectivePermissionsAsync(roleList, ct);
        return effective.Contains(DinardapPermissions.AdminAccess) || effective.Contains(permissionCode);
    }

    private async Task<HashSet<string>> GetEffectivePermissionsAsync(List<string> roles, CancellationToken ct)
    {
        var cacheKey = $"dinardap_effective_perms_{string.Join(',', roles)}";
        if (_cache.TryGetValue<HashSet<string>>(cacheKey, out var cached) && cached is not null)
            return cached;

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var query = string.Join("&", roles.Select(r => $"roles={Uri.EscapeDataString(r)}"));
            var url = $"{_options.Url.TrimEnd('/')}/{_options.EffectivePermissionsUrl.TrimStart('/')}?{query}";

            var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("WsSegu respondio {Status} al consultar permisos efectivos para roles {Roles}", response.StatusCode, string.Join(',', roles));
                return new HashSet<string>();
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            // ApiResponse.Ok(list) de WsSegu: { success, data: [ { module, action, ... } ], message }
            var data = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;

            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (data.ValueKind == JsonValueKind.Array)
            {
                foreach (var perm in data.EnumerateArray())
                {
                    var module = perm.TryGetProperty("module", out var m) ? m.GetString() : null;
                    var action = perm.TryGetProperty("action", out var a) ? a.GetString() : null;
                    if (!string.IsNullOrEmpty(module) && !string.IsNullOrEmpty(action))
                        codes.Add($"{module}.{action}".ToUpperInvariant());
                }
            }

            _cache.Set(cacheKey, codes, TimeSpan.FromMinutes(_options.PermissionsCacheMinutes));
            return codes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando permisos efectivos en WsSegu para roles {Roles}", string.Join(',', roles));
            return new HashSet<string>();
        }
    }
}

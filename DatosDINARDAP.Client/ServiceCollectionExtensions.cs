using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DatosDINARDAP.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra IDinardapClient sobre HttpClientFactory. El consumidor debe registrar su
    /// propia implementacion de IDinardapTokenProvider antes de llamar a este metodo.
    /// </summary>
    public static IServiceCollection AddDinardapClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DinardapClientOptions>(configuration.GetSection(DinardapClientOptions.SectionName));
        services.AddTransient<BearerTokenHandler>();

        services.AddHttpClient<IDinardapClient, DinardapClient>((sp, http) =>
            {
                var options = sp.GetRequiredService<IOptions<DinardapClientOptions>>().Value;
                http.BaseAddress = new Uri(options.BaseUrl);
                http.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .AddHttpMessageHandler<BearerTokenHandler>();

        return services;
    }
}

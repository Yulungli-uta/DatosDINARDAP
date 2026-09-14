using System.Configuration;

namespace DatosDINARDAP.Datos.Infrastructure
{
    /// <summary>
    /// Configuracion leida de app.config (appSettings). Sin credenciales: WsUtaDinardap.Api
    /// confia esta llamada por IP de origen (LegacyNetworkTrust en el servidor), no por
    /// token - esta DLL no necesita ClientId/ClientSecret ni hablar con WsSegu.
    /// </summary>
    internal static class WsUtaDinardapApiOptions
    {
        public static string BaseUrl =>
            ConfigurationManager.AppSettings["WsUtaDinardapApi.BaseUrl"] ?? "https://localhost:5001";

        public static int TimeoutSeconds
        {
            get
            {
                var raw = ConfigurationManager.AppSettings["WsUtaDinardapApi.TimeoutSeconds"];
                return int.TryParse(raw, out var seconds) && seconds > 0 ? seconds : 30;
            }
        }
    }
}

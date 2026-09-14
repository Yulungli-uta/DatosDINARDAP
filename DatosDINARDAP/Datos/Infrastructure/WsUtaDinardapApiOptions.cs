using System;
using System.Configuration;

namespace DatosDINARDAP.Datos.Infrastructure
{
    /// <summary>
    /// Configuracion leida de app.config (appSettings) + variables de entorno.
    /// El ClientSecret NUNCA se lee de app.config en produccion (queda vacio si no hay
    /// variable de entorno) - a proposito, para no repetir el problema de credenciales
    /// hardcodeadas que tenia esta misma DLL contra DINARDAP directo.
    /// </summary>
    internal static class WsUtaDinardapApiOptions
    {
        public static string BaseUrl =>
            ConfigurationManager.AppSettings["WsUtaDinardapApi.BaseUrl"] ?? "https://localhost:5001";

        /// <summary>URL de WsSegu (RepositoryUta) para obtener el token tecnico via Client Credentials.</summary>
        public static string AuthServiceUrl =>
            ConfigurationManager.AppSettings["WsUtaDinardapApi.AuthServiceUrl"] ?? "http://localhost:5010";

        public static string ClientId =>
            ConfigurationManager.AppSettings["WsUtaDinardapApi.ClientId"] ?? string.Empty;

        /// <summary>
        /// Variable de entorno WSUTADINARDAP_CLIENT_SECRET tiene prioridad siempre.
        /// Solo si no existe, se intenta appSettings (unicamente para desarrollo local -
        /// nunca debe llevar un valor real en un app.config versionado).
        /// </summary>
        public static string ClientSecret =>
            Environment.GetEnvironmentVariable("WSUTADINARDAP_CLIENT_SECRET")
            ?? ConfigurationManager.AppSettings["WsUtaDinardapApi.ClientSecret.DevOnly"]
            ?? string.Empty;

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

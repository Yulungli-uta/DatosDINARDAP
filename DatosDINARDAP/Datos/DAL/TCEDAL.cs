using System;

using DatosDINARDAP.Datos.Infrastructure;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class TCEDAL
    {
        string Codigo;
        string Servicio;

        /// <summary>
        /// Constructor de la clase TCEDAL
        /// </summary>

        public TCEDAL()
        {
            Codigo = "";
            Servicio = "114";
        }

        public TCEDAL(string codigo)
        {
            Codigo = codigo;
            Servicio = "114";
        }

        public TCEDAL(string codigo, string servicio)
        {
            Codigo = codigo;
            Servicio = servicio;
        }

        /// <summary>
        /// Consumo de WsUtaDinardap.Api (GET /api/v1/dinardap/tce/{id}). El contrato de TCE
        /// nunca tuvo campo de error (ver matriz de compatibilidad del analisis) - se preserva
        /// eso exactamente: exito, sin-datos y falla se ven igual (objeto con defaults) para
        /// no romper consumidores que ya conviven con esa ambiguedad historica. sufrago=null
        /// (API nueva, honesto) se colapsa aqui a false para reproducir el default de siempre.
        /// </summary>
        public TCE getDatosTCE()
        {
            var datos = new TCE();

            try
            {
                var dto = DinardapApiClient.GetAsync<TceApiDto>(
                    "/api/v1/dinardap/tce/" + Uri.EscapeDataString(Codigo)).GetAwaiter().GetResult();

                datos.numeroCertificado = dto.NumeroCertificado;
                datos.fechaSufragio = dto.FechaSufragio ?? default(DateTime);
                datos.sufrago = dto.Sufrago ?? false;
            }
            catch (Exception)
            {
                // Igual que el comportamiento historico: no hay campo de error en TCE, la
                // falla se ve identica a "sin datos" (objeto con valores default).
                datos = new TCE();
            }

            return datos;
        }
    }
}

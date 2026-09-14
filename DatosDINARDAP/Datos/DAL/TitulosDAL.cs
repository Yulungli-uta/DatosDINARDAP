using System;
using System.Collections.Generic;
using System.Linq;

using DatosDINARDAP.Datos.Infrastructure;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class TitulosDAL
    {
        string Codigo;
        string Servicio;

        /// <summary>
        /// Constructor de la clase TitulosDAL
        /// </summary>

        public TitulosDAL()
        {
            Codigo = "";
            Servicio = "115";
        }

        public TitulosDAL(string codigo)
        {
            Codigo = codigo;
            Servicio = "115";
        }

        public TitulosDAL(string codigo, string servicio)
        {
            Codigo = codigo;
            Servicio = servicio;
        }

        /// <summary>
        /// Consumo de WsUtaDinardap.Api (GET /api/v1/dinardap/titulos/{id}). Preserva el
        /// contrato historico: nunca null (lista vacia si no hay titulos o si la llamada
        /// falla), sin campo de error. Fechas sin dato honesto (null en la API nueva) se
        /// colapsan aqui al centinela historico 1900-01-01.
        /// </summary>
        public List<Titulos> getDatosTitulos()
        {
            var lista = new List<Titulos>();

            try
            {
                var dtos = DinardapApiClient.GetAsync<List<TituloApiDto>>(
                    "/api/v1/dinardap/titulos/" + Uri.EscapeDataString(Codigo)).GetAwaiter().GetResult();

                if (dtos != null)
                {
                    lista = dtos.Select(dto => new Titulos
                    {
                        nivelNombre = dto.NivelNombre,
                        nivel = dto.Nivel,
                        fechaGrado = dto.FechaGrado ?? new DateTime(1900, 1, 1),
                        fechaRegistro = dto.FechaRegistro ?? new DateTime(1900, 1, 1),
                        institucionEducacionSuperior = dto.InstitucionEducacionSuperior,
                        nombreTitulo = dto.NombreTitulo,
                        numeroRegistro = dto.NumeroRegistro,
                        tipo = dto.Tipo,
                        tipoExtranjeroColegio = dto.TipoExtranjeroColegio
                    }).ToList();
                }
            }
            catch (Exception)
            {
                // Igual que el comportamiento historico: sin campo de error, cualquier falla
                // se ve identica a "sin titulos" (lista vacia, nunca null).
                lista = new List<Titulos>();
            }

            return lista;
        }
    }
}

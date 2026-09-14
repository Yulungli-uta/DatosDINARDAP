using System;

using DatosDINARDAP.Datos.Infrastructure;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class RegistroCivilDAL
    {
        string Codigo;
        string Servicio;

        /// <summary>
        /// Constructor de la clase RegistroCivilDAL
        /// </summary>

        public RegistroCivilDAL()
        {
            Codigo = "";
            Servicio = "113";
        }

        public RegistroCivilDAL(string codigo)
        {
            Codigo = codigo;
            Servicio = "113";
        }

        public RegistroCivilDAL(string codigo, string servicio)
        {
            Codigo = codigo;
            Servicio = servicio;
        }

        /// <summary>
        /// Consumo de WsUtaDinardap.Api (GET /api/v1/dinardap/registro-civil/{id}) en vez de
        /// SOAP directo a DINARDAP. Mismo contrato publico y mismo comportamiento de error que
        /// antes: nunca lanza, siempre retorna un RegistroCivil (error=0 exito/sin datos, error=1
        /// ante cualquier falla).
        /// </summary>
        public RegistroCivil getDatosRegistroCivil()
        {
            var datos = new RegistroCivil();

            try
            {
                var dto = DinardapApiClient.GetAsync<RegistroCivilApiDto>(
                    "/api/v1/dinardap/registro-civil/" + Uri.EscapeDataString(Codigo)).GetAwaiter().GetResult();

                datos.codigo = dto.Codigo;
                datos.nombreCompleto = dto.NombreCompleto;
                datos.nombres = dto.Nombres;
                datos.apellido1 = dto.Apellido1;
                datos.apellido2 = dto.Apellido2;
                datos.genero = dto.Genero;
                datos.condicionCiudadano = dto.CondicionCiudadano;
                datos.fechaNacimiento = dto.FechaNacimiento ?? default(DateTime);
                datos.lugarNacimiento = dto.LugarNacimiento;
                datos.lugarNacimientoProvincia = dto.LugarNacimientoProvincia;
                datos.lugarNacimientoCiudad = dto.LugarNacimientoCiudad;
                datos.lugarNacimientoParroquia = dto.LugarNacimientoParroquia;
                datos.nacionalidad = dto.Nacionalidad;
                datos.estadoCivil = dto.EstadoCivil;
                datos.conyuge = dto.Conyuge;
                datos.nombrePadre = dto.NombrePadre;
                datos.nombreMadre = dto.NombreMadre;
                datos.error = 0;
            }
            catch (Exception)
            {
                // Igual que el comportamiento historico: cualquier falla (red, SOAP/HTTP,
                // credenciales, timeout) se traduce a error=1 con el resto de campos vacios.
                datos = new RegistroCivil { error = 1 };
            }

            return datos;
        }
    }
}

using System;
using System.Collections.Generic;

namespace DatosDINARDAP.Datos.Infrastructure
{
    // Forma de la respuesta JSON de WsUtaDinardap.Api. Newtonsoft.Json matchea nombres de
    // propiedad sin distinguir mayusculas/minusculas por default, asi que estas clases en
    // PascalCase deserializan bien el camelCase que devuelve ASP.NET Core.

    internal sealed class RegistroCivilApiDto
    {
        public string Codigo { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombres { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Genero { get; set; }
        public string CondicionCiudadano { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string LugarNacimiento { get; set; }
        public string LugarNacimientoProvincia { get; set; }
        public string LugarNacimientoCiudad { get; set; }
        public string LugarNacimientoParroquia { get; set; }
        public string Nacionalidad { get; set; }
        public string EstadoCivil { get; set; }
        public string Conyuge { get; set; }
        public string NombrePadre { get; set; }
        public string NombreMadre { get; set; }
    }

    internal sealed class TceApiDto
    {
        public string NumeroCertificado { get; set; }
        public DateTime? FechaSufragio { get; set; }
        public bool? Sufrago { get; set; }
    }

    internal sealed class TituloApiDto
    {
        public string NivelNombre { get; set; }
        public int Nivel { get; set; }
        public DateTime? FechaGrado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string InstitucionEducacionSuperior { get; set; }
        public string NombreTitulo { get; set; }
        public string NumeroRegistro { get; set; }
        public string Tipo { get; set; }
        public string TipoExtranjeroColegio { get; set; }
    }

    internal sealed class DinardapProblemDetailsDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Code { get; set; }
        public string TraceId { get; set; }
    }
}

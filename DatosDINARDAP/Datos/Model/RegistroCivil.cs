using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatosDINARDAP.Datos.Model
{
    public class RegistroCivil
    {
        public string codigo { get; set; }
        public string nombreCompleto { get; set; }
        public string nombres { get; set; }
        public string apellido1 { get; set; }
        public string apellido2 { get; set; }
        public string genero { get; set; }
        public string condicionCiudadano { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string lugarNacimiento { get; set; }
        public string lugarNacimientoProvincia { get; set; }
        public string lugarNacimientoCiudad { get; set; }
        public string lugarNacimientoParroquia { get; set; }
        public string nacionalidad { get; set; }
        public string estadoCivil { get; set; }
        public string conyuge { get; set; }
        public string nombrePadre { get; set; }
        public string nombreMadre { get; set; }
        public int error { get; set; }
        
    }
}

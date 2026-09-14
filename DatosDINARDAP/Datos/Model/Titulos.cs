using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatosDINARDAP.Datos.Model
{
    public class Titulos
    {
        public string nivelNombre { get; set; }
        public int nivel { get; set; }
        public DateTime fechaGrado { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string institucionEducacionSuperior { get; set; }
        public string nombreTitulo { get; set; }
        public string numeroRegistro { get; set; }
        public string tipo { get; set; }
        public string tipoExtranjeroColegio { get; set; }
        //public int error { get; set; }

    }
}

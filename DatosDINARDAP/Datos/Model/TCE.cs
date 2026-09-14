using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatosDINARDAP.Datos.Model
{
    public class TCE
    {
        public string numeroCertificado { get; set; }
        public DateTime fechaSufragio { get; set; }
        public bool sufrago { get; set; }
        //public int error { get; set; }
    }
}

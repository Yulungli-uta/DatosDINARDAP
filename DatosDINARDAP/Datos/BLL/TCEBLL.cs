using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatosDINARDAP.Datos.Model;
using DatosDINARDAP.Datos.DAL;


namespace DatosDINARDAP.Datos.BLL
{
    class TCEBLL
    {
        /// <summary>
        /// Obtiene datos de TCE
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <param name="servicio">Código del servicio</param>
        /// <returns>Registro con datos del ciudadano en TCE</returns>
        public TCE getDatosTCEAll(string codigo, string servicio)
        {
            TCEDAL objTCEDAL = new TCEDAL(codigo, servicio);
            return objTCEDAL.getDatosTCE();
        }

        /// <summary>
        /// Obtiene datos de registro civil
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <returns>Registro con datos del ciudadano en TCE</returns>
        public TCE getDatosTCEAll(string codigo)
        {
            TCEDAL objTCEDAL = new TCEDAL(codigo);
            return objTCEDAL.getDatosTCE();
        }
    }
}

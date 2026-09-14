using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DatosDINARDAP.Datos.Model;
using DatosDINARDAP.Datos.DAL;

namespace DatosDINARDAP.Datos.BLL
{
    class RegistroCivilBLL
    {
        /// <summary>
        /// Obtiene datos de registro civil
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <param name="servicio">Código del servicio</param>
        /// <returns>Registro con datos del ciudadano</returns>
        public RegistroCivil getDatosRegistroCivilAll(string codigo, string servicio)
        {
            RegistroCivilDAL objRegistroCivilDAL = new RegistroCivilDAL(codigo, servicio);
            return objRegistroCivilDAL.getDatosRegistroCivil();
        }

        /// <summary>
        /// Obtiene datos de registro civil
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <returns>Registro con datos del ciudadano</returns>
        public RegistroCivil getDatosRegistroCivilAll(string codigo)
        {
            RegistroCivilDAL objRegistroCivilDAL = new RegistroCivilDAL(codigo);
            return objRegistroCivilDAL.getDatosRegistroCivil();
        }
    }
}

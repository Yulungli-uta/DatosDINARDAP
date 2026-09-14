using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatosDINARDAP.Datos.Model;
using DatosDINARDAP.Datos.BLL;

namespace DatosDINARDAP
{
    public class ConsultaDatos
    {
        /// <summary>
        /// Obtiene datos de Registro Civil
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <returns>Registro con datos del ciudadano</returns>
        /// 

        public RegistroCivil DatosRegistroCivil(string codigo)
        {
            RegistroCivilBLL objRegistroCivilBLL = new RegistroCivilBLL();
            return objRegistroCivilBLL.getDatosRegistroCivilAll(codigo);
        }

        /// <summary>
        /// Obtiene datos de la SENESCYT
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <returns>Registro de los títulos del ciudadano</returns>
        /// 
        public List<Titulos> DatosTitulos(string codigo)
        {
            TitulosBLL objTitulosBLL = new TitulosBLL();
            return objTitulosBLL.getDatosTitulosAll(codigo);
        }

        /// <summary>
        /// Obtiene datos de TCE
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>        
        /// <returns>Registro con datos del ciudadano en TCE</returns>
        /// 
        public TCE DatosTCE(string codigo)
        {
            TCEBLL objTCEBLL = new TCEBLL();
            return objTCEBLL.getDatosTCEAll(codigo);
        }
    }
}

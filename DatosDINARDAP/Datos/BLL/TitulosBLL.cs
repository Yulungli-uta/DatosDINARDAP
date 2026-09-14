using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatosDINARDAP.Datos.Model;
using DatosDINARDAP.Datos.DAL;


namespace DatosDINARDAP.Datos.BLL
{
    class TitulosBLL
    {
        
        /// <summary>
        /// Obtiene los datos de los titulos registrados en la SENESCYT
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <param name="servicio">Código del servicio</param>
        /// <returns>Registros de los titulos con datos del ciudadano</returns>
        /// 
        public List<Titulos> getDatosTitulosAll(string codigo, string servicio)
        {
            TitulosDAL objTitulosBLL = new TitulosDAL(codigo, servicio);
            return objTitulosBLL.getDatosTitulos();
        }

        /// <summary>
        /// Obtiene los datos de los titulos registrados en la SENESCYT
        /// </summary>
        /// <param name="codigo">Código del ciudadano</param>
        /// <returns>Registros de los titulos con datos del ciudadano</returns>
        /// 
        public List<Titulos> getDatosTitulosAll(string codigo)
        {
            TitulosDAL objTitulosBLL = new TitulosDAL(codigo);
            return objTitulosBLL.getDatosTitulos();
        }
    }
}

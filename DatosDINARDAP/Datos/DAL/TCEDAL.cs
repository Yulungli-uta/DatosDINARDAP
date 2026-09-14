using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatosDINARDAP.ServiceReferenceDINARDAP;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class TCEDAL
    {
        string Codigo;
        string Servicio;
        
        /// <summary>
        /// Construrtor de la clase TCEDAL
        /// </summary>
        
        public TCEDAL()
        {
            Codigo = "";
            Servicio = "114";
        }

        public TCEDAL(string codigo)
        {
            Codigo = codigo;
            Servicio = "114";
        }

        public TCEDAL(string codigo, string servicio)
        {
            Codigo = codigo;
            Servicio = servicio;
        }

        /// <summary>
        /// Consumo web service TCE
        /// </summary>
        /// <returns>registro TCE</returns>

        public TCE getDatosTCE()
        {
            TCE datos = new TCE();

            InteroperadorClient cliente = new InteroperadorClient();
            cliente.ClientCredentials.UserName.UserName = "REDACTED_ROTATE_WITH_DINARDAP";
            cliente.ClientCredentials.UserName.Password = "REDACTED_ROTATE_WITH_DINARDAP";

            fichaGeneral ficha;
            string resultado;
            try
            {
                ficha = cliente.getFichaGeneral(Codigo, Servicio);

                resultado = "";
                for (int k = 0; k < ficha.instituciones.Length; k++)
                    for (int i = 0; i < ficha.instituciones[k].datosPrincipales.Length; i++)
                    {
                        switch (ficha.instituciones[k].datosPrincipales[i].codigo)
                        {
                            case "44": //Número de certificado
                                datos.numeroCertificado = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "45": //fecha que se realizó la votación yyyy/MM/dd
                                datos.fechaSufragio = new DateTime(Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(0, 4)), Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(5, 2)), Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(8, 2)));
                                break;
                            case "46": //nacionalidad
                                if (ficha.instituciones[k].datosPrincipales[i].valor.Equals("SI"))
                                    datos.sufrago = true;
                                else
                                    datos.sufrago = false;
                                break;                            
                            default:
                                break;
                        }
                    }

            }
            catch (Exception ex)
            {
                resultado = "Error " + ex;
            }

            return datos;
        }
    }
}

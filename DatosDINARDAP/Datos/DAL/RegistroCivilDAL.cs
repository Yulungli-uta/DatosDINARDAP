using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.ComponentModel;
//using System.Data;

using DatosDINARDAP.ServiceReferenceDINARDAP;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class RegistroCivilDAL
    {
        string Codigo;
        string Servicio;
        
        /// <summary>
        /// Construrtor de la clase RegistroCivilDAL
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
        /// Consumo web service registro civil
        /// </summary>
        /// <returns></returns>
        public RegistroCivil getDatosRegistroCivil()
        {
            RegistroCivil datos = new RegistroCivil();

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
                            case "1": //Cedula
                                datos.codigo = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "2": //nombre
                                string nombre = SepararApellidos(ficha.instituciones[k].datosPrincipales[i].valor);
                                datos.nombreCompleto = nombre.Replace('@', ' ');
                                datos.apellido1 = "";
                                    datos.apellido2 = "";
                                    datos.nombres = "";
                                string[] nombres = nombre.Split('@');

                                if (nombres.Length == 1)
                                {
                                    datos.apellido1 = nombres[0];
                                }
                                else if (nombres.Length == 2)
                                {
                                    datos.apellido1 = nombres[0];
                                    datos.nombres = nombres[1];
                                }
                                else if (nombres.Length >= 3)
                                {
                                    datos.apellido1 = nombres[0];
                                    datos.apellido2 = nombres[1];
                                    for (int ii = 2; ii < nombres.Length; ii++)
                                        datos.nombres += (datos.nombres.Length == 0 ? "" : " ") + nombres[ii];
                                }
                                
                                break;
                            case "3": //género
                                datos.genero = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "4": //Condición ciudadano 
                                datos.condicionCiudadano = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "5": //fecha nacimiento formato origen dd/mm/yyyy
                                datos.fechaNacimiento = new DateTime(Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(6, 4)), Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(3, 2)), Convert.ToInt16(ficha.instituciones[k].datosPrincipales[i].valor.Substring(0, 2)));
                                break;
                            case "6": //lugarnacimiento
                                datos.lugarNacimiento = ficha.instituciones[k].datosPrincipales[i].valor;
                                string[] lugar = datos.lugarNacimiento.Split('/');
                                if (lugar.Length > 0)
                                    datos.lugarNacimientoProvincia = lugar[0];
                                if (lugar.Length > 1)
                                    datos.lugarNacimientoCiudad = lugar[1];
                                if (lugar.Length > 2)
                                    datos.lugarNacimientoParroquia = lugar[2];
                                break;
                            case "7": //nacionalidad
                                datos.nacionalidad = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "8": //estado civil
                                datos.estadoCivil = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "10": //conyugue
                                datos.conyuge = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "11": //nombre padre
                                datos.nombrePadre = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            case "13": //nombre madre
                                datos.nombreMadre = ficha.instituciones[k].datosPrincipales[i].valor;
                                break;
                            default: 
                                break;
                        }
                    }
                datos.error = 0;
            }
            catch (Exception ex)
            {
                resultado = "Error " + ex;
                datos.error = 1;
            }

            return datos;
        }

        private string SepararApellidos(string nombreCompleto)
        {
            string[] nombreArray = nombreCompleto.Split(' ');
            string nombreSeparado = "";
            for (int i = 0; i < nombreArray.Length; i++)
            {
                if (nombreArray[i].Length > 0)
                {
                    switch (nombreArray[i].ToLower())
                    {
                        case "de":
                        case "del":
                        case "la":
                        case "las":
                        case "los":
                        case "san":
                            nombreSeparado += nombreArray[i] + " ";
                            break;
                        default:
                            nombreSeparado += nombreArray[i] + "@";
                            break;
                    }
                }
            }

            if (nombreSeparado.Substring(nombreSeparado.Length - 1, 1) == "@")
                nombreSeparado = nombreSeparado.Remove(nombreSeparado.Length - 1);
            return nombreSeparado;
        }
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DatosDINARDAP.ServiceReferenceDINARDAP;
using DatosDINARDAP.Datos.Model;

namespace DatosDINARDAP.Datos.DAL
{
    class TitulosDAL
    {
        string Codigo;
        string Servicio;
        
        /// <summary>
        /// Construrtor de la clase TitulosDAL
        /// </summary>
        
        public TitulosDAL()
        {
            Codigo = "";
            Servicio = "115";
        }

        public TitulosDAL(string codigo)
        {
            Codigo = codigo;
            Servicio = "115";
        }

        public TitulosDAL(string codigo, string servicio)
        {
            Codigo = codigo;
            Servicio = servicio;
        }

        /// <summary>
        /// Consumo web service SENESCYT
        /// </summary>
        /// <returns>Lista de Titulos</returns>
        public List<Titulos> getDatosTitulos()
        {
            List<Titulos> lista = new List<Titulos>();


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
                    for (int i = 0; i < ficha.instituciones[k].detalle.Length; i++)
                    {
                        Titulos objTitulo = new Titulos();
                        objTitulo.nivelNombre = ficha.instituciones[k].detalle[i].nombre;
                        if (ficha.instituciones[k].detalle[i].nombre.Equals("Títulos de Tercer Nivel")
                            || ficha.instituciones[k].detalle[i].nombre.Equals("Tercer Nivel o Pregrado"))
                            objTitulo.nivel = 3;
                        else if (ficha.instituciones[k].detalle[i].nombre.Equals("Títulos Cuarto Nivel")
                            || ficha.instituciones[k].detalle[i].nombre.Equals("Cuarto Nivel o Posgrado")
                            || ficha.instituciones[k].detalle[i].nombre.Equals("CUARTO_NIVEL"))
                            objTitulo.nivel = 4;
                        else
                            objTitulo.nivel = 2;

                        for (int l = 0; l < ficha.instituciones[k].detalle[i].registros.Length; l++)
                            switch (ficha.instituciones[k].detalle[i].registros[l].codigo)
                            {

                                case "51": //fecha fecha de grado yyyy/mm/dd
                                    if (ficha.instituciones[k].detalle[i].registros[l].valor != null)
                                        if (ficha.instituciones[k].detalle[i].registros[l].valor.Length >= 8)
                                            objTitulo.fechaGrado = new DateTime(Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(0, 4)), Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(5, 2)), Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(8, 2)));
                                        else
                                            objTitulo.fechaGrado = new DateTime(1900, 1, 1);
                                    else
                                        objTitulo.fechaGrado = new DateTime(1900, 1, 1);
                                    break;
                                case "52": //fecha fecha de registro yyyy/mm/dd
                                    if (ficha.instituciones[k].detalle[i].registros[l].valor != null)
                                        if (ficha.instituciones[k].detalle[i].registros[l].valor.Length >= 8)
                                            objTitulo.fechaRegistro = new DateTime(Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(0, 4)), Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(5, 2)), Convert.ToInt16(ficha.instituciones[k].detalle[i].registros[l].valor.Substring(8, 2)));
                                        else
                                            objTitulo.fechaRegistro = new DateTime(1900, 1, 1);
                                    else
                                        objTitulo.fechaRegistro = new DateTime(1900, 1, 1);
                                    break;
                                case "53": //Nombre de la institución en la cual obtuvo el título
                                    objTitulo.institucionEducacionSuperior = ficha.instituciones[k].detalle[i].registros[l].valor;
                                    break;
                                case "54": //nombre del título
                                    objTitulo.nombreTitulo = ficha.instituciones[k].detalle[i].registros[l].valor;
                                    break;
                                case "55": //número de registro
                                    objTitulo.numeroRegistro = ficha.instituciones[k].detalle[i].registros[l].valor;
                                    break;
                                case "56": //tipo {Nacionales}
                                    objTitulo.tipo = ficha.instituciones[k].detalle[i].registros[l].valor;
                                    break;
                                case "57": //tipo Extranjero Colegio {?}
                                    objTitulo.tipoExtranjeroColegio = ficha.instituciones[k].detalle[i].registros[l].valor;
                                    break;
                                default:
                                    break;
                            }
                        //verifica si está ingresado codigo de registro en la lista
                        //bool datoinsertado = false;
                        //foreach (Titulos tit in lista)
                        //    if (tit.numeroRegistro.Equals(objTitulo.numeroRegistro))
                        //    {
                        //        datoinsertado = true;
                        //        break;
                        //    }
                        //if (!datoinsertado)
                            lista.Add(objTitulo);
                    }

            }
            catch (Exception ex)
            {
                resultado = "Error " + ex;
            }

            return lista;
        }
    }
}

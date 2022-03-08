using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace ReportesWeb1_2.Services
{
    public class MetodosGlbRepository
    {
        private static OracleConnection Conexion = new OracleConnection();

        public DataSet Ds = new DataSet();
        public DataSet Ds1 = new DataSet();
        public DataSet Ds2 = new DataSet();
        public DataSet Ds3 = new DataSet();
        public DataSet Ds4 = new DataSet();
        public DataSet Ds5 = new DataSet();
        public DataSet Ds6 = new DataSet();
        public DataSet Ds7 = new DataSet();
        public DataSet Ds8 = new DataSet();
        public DataSet DsTarifa = new DataSet();
        public DataRow oDataRow;
        public DataRow oDataRow1;
        public DataRow oDataRow2;
        public DataRow oDataRow3;
        public DataRow oDataRow4;
        public DataRow oDataRow5;
        public DataRow oDataRow6;
        public DataRow oDataRow7;
        public DataRow oDataRow8;
        public DataRow oDataRowTarifa;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        /// <summary>
        /// Abre la conexión Oracle y retorna un OracleConnection.
        /// </summary>
        /// <returns></returns>
        public OracleConnection GetConnectionOracle(string NameConString)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\Log\ErroresAdmin.txt", true);

                sw.WriteLine("Error en el getConnectionOracle");
                
                if (Conexion.State == ConnectionState.Closed)
                {
                    sw.WriteLine("Paso el if");

                    string ConnectString = ConfigurationManager.ConnectionStrings[NameConString].ConnectionString;
                    Conexion.ConnectionString = ConnectString;
                    Conexion.Open();

                    sw.WriteLine(ConnectString);
                }

                sw.WriteLine("Salimos del if y la conexion fue buena");
                sw.Flush();
                sw.Close();

                return Conexion;
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(@"C:\Log\ErroresAdmin.txt", true);

                sw.WriteLine("Error en el getConnectionOracle");
                sw.WriteLine(ex.ToString());
                sw.Flush();
                sw.Close();

                throw;
            }
        }

        /// <summary>
        /// Cierra la conexión Oracle.
        /// </summary>
        /// <returns></returns>
        public void CloseConnectionOracle()
        {
            if (Conexion.State == ConnectionState.Open)
                Conexion.Close();
        }

        /// <summary>
        /// Lee archivo .ini.
        /// </summary>
        /// <param name="Ruta"></param>
        /// <param name="Seccion"></param>
        /// <param name="Variable"></param>
        /// <returns></returns>
        public string LeeINI(string Ruta, string Seccion, string Variable)
        {
            string Res;
            try
            {

                StringBuilder Resultado;
                Resultado = new StringBuilder((char)0, 255);

                uint Caracteres;
                Caracteres = GetPrivateProfileString(Seccion, Variable, "", Resultado, 255, Ruta);

                Res = Left(Convert.ToString(Resultado), Convert.ToInt32(Caracteres));
            }
            catch (Exception ex)
            {
                Res = string.Empty;
            }
            return Res;
        }

        /// <summary>
        /// Devuelve una cadena que contiene un número especificado de caracteres a partir del lado izquierdo de una cadena.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(string param, int length)
        {
            string result = param.Substring(0, length);

            return result;
        }

        /// <summary>
        /// Funcion creada con base a VB para los if comprimidos. 
        /// </summary>
        /// <param name="Expression"></param>
        /// <param name="TruePart"></param>
        /// <param name="FalsePart"></param>
        /// <returns></returns>
        public string IIf(bool Expression, string TruePart, string FalsePart)
        {
            string ReturnValue = Expression == true ? TruePart : FalsePart;

            return ReturnValue;
        }

        /// <summary>
        /// Función creada con base a VB para retornar un booleano si la expresión se puede evaluar en númerico.
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public Boolean IsNumeric(string valor)
        {
            return int.TryParse(valor, out int result);
        }

        /// <summary>
        /// Convierte una cadena a un formato dd/MM/yyyy. 
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public string Fecha(string fecha)
        {
            string _fecha = fecha.Substring(6, 2) + "/" + fecha.Substring(4, 2) + "/" + fecha.Substring(0, 4) + " " + fecha.Substring(8, 2) + ":" + fecha.Substring(10, 2) + ":" + fecha.Substring(12, 2);

            return _fecha;
        }

        /// <summary>
        /// Método 0 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds.Tables.Count != 0)
                Ds.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds, Nombre_tabla);
                    try
                    {
                        if (Ds.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow = Ds.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }

                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 1 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet1(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds1.Tables.Count != 0)
                Ds1.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds1, Nombre_tabla);
                    try
                    {
                        if (Ds1.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow1 = Ds1.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }

                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 2 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet2(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds2.Tables.Count != 0)
                Ds2.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds2, Nombre_tabla);
                    try
                    {
                        if (Ds2.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow2 = Ds2.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 3 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet3(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds3.Tables.Count != 0)
                Ds3.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds3, Nombre_tabla);
                    try
                    {
                        if (Ds3.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow3 = Ds3.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 4 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>

        public bool QueryDataSet4(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds4.Tables.Count != 0)
                Ds4.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds4, Nombre_tabla);
                    try
                    {
                        if (Ds4.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow4 = Ds4.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 5 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet5(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds5.Tables.Count != 0)
                Ds5.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds5, Nombre_tabla);
                    try
                    {
                        if (Ds5.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow5 = Ds5.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }


        /// <summary>
        /// Método 6 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet6(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds6.Tables.Count != 0)
                Ds6.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds6, Nombre_tabla);
                    try
                    {
                        if (Ds6.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow6 = Ds6.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 7 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet7(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds7.Tables.Count != 0)
                Ds7.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds7, Nombre_tabla);
                    try
                    {
                        if (Ds7.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow7 = Ds7.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método 8 para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSet8(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (Ds8.Tables.Count != 0)
                Ds8.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(Ds8, Nombre_tabla);
                    try
                    {
                        if (Ds8.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRow8 = Ds8.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Método Tarifa para ejecutrar un query y adaptarlo a un DataSet especificando la nombre de la tabla.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Nombre_tabla"></param>
        /// <returns></returns>
        public bool QueryDataSetTarifa(string Query, string Nombre_tabla)
        {
            bool _return = false;

            if (DsTarifa.Tables.Count != 0)
                DsTarifa.Clear();

            using (OracleCommand Cmd = new OracleCommand(Query, Conexion))
            {
                using (OracleDataAdapter Da = new OracleDataAdapter(Cmd))
                {
                    Da.Fill(DsTarifa, Nombre_tabla);
                    try
                    {
                        if (DsTarifa.Tables[Nombre_tabla].Rows.Count > 0)
                        {
                            _return = true;
                            oDataRowTarifa = DsTarifa.Tables[Nombre_tabla].Rows[0];
                        }
                        else
                            _return = false;
                    }
                    catch (Exception ex)
                    {
                        _return = false;
                    }
                    finally
                    {
                        Cmd.Dispose();
                    }
                }
            }

            return _return;
        }

        /// <summary>
        /// Agregar columna a un DataTable, dando formato 
        /// </summary>
        /// <param name="columna"></param>
        /// <param name="int_tipo"></param>
        /// <param name="Nombre_colum"></param>
        public DataColumn Agregar_datacolum(int int_tipo, string Nombre_colum)
        {
            DataColumn columna = new DataColumn();

            switch (int_tipo)
            {
                case 1: //double
                    columna = new DataColumn();
                    columna.DataType = System.Type.GetType("System.Double");
                    columna.ColumnName = Nombre_colum;
                    columna.ReadOnly = true;
                    columna.Unique = false;
                    break;
                case 2: //string
                    columna = new DataColumn();
                    columna.DataType = System.Type.GetType("System.String");
                    columna.ColumnName = Nombre_colum;
                    columna.ReadOnly = true;
                    columna.Unique = false;
                    break;
                case 3: //date
                    columna = new DataColumn();
                    columna.DataType = System.Type.GetType("System.DateTime");
                    columna.ColumnName = Nombre_colum;
                    columna.ReadOnly = true;
                    columna.Unique = false;
                    break;
                default:
                    break;
            }

            return columna;
        }

        /// <summary>
        /// asignacion_excedente_clase (Código viejo)
        /// </summary>
        /// <param name="id_clase"></param>
        /// <returns></returns>
        public string Asignacion_excedente_clase(int id_clase)
        {
            string rpt = string.Empty;
            switch (id_clase)
            {
                //10, 11, 17
                case int n when (n == 10 || n == 11 || n == 17):
                    rpt = "01";
                    break;
                //16, 18
                case int n when (n == 16 || n == 18):
                    rpt = "09";
                    break;
                default:
                    break;
            }

            return rpt;
        }
    }
}
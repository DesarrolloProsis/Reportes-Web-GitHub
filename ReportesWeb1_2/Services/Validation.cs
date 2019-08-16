using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReportesWeb1_2.Services
{
    public class Validation
    {
        bool BanValidaciones = true;
        bool BanValidaciones2 = false;
        bool BanValidaciones3 = false;
        public List<filas> listass = new List<filas>();
        public string Message = string.Empty;
        bool Null = false;

        /// <summary>
        /// Validar carriles cerrados
        /// </summary>
        /// <param name="FechaInicioD"></param>
        /// <param name="FechaSelect"></param>
        /// <param name="TempTurno"></param>
        /// <returns></returns>
        public string ValidarCarrilesCerrados(DateTime FechaInicioD, DateTime FechaSelect, string TempTurno, string Conexion)
        {
            Carril Carril = new Carril();
            OracleCommand Cmd = new OracleCommand();
            OracleConnection Connection = new OracleConnection(Conexion);
            List<Carril> Carriles = new List<Carril>();
            List<Carril> CarrilesCerrados = new List<Carril>();
            //Connection.ConnectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            string rpt = string.Empty;
            string TurnoP = string.Empty;
            string FechaInicio = string.Empty;
            string FechaFinal = string.Empty;
            BanValidaciones = true;

            switch (TempTurno.Substring(0, 2))
            {
                case "06":
                    TurnoP = "2";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 06:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 13:59:59";
                    break;
                case "14":
                    TurnoP = "3";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 14:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 21:59:59";
                    break;
                case "22":
                    TurnoP = "1";
                    FechaInicio = FechaInicioD.AddDays(-1).ToString("MM/dd/yyyy") + " 22:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 05:59:59";
                    break;
            }

            string Query = @"SELECT	LANE_ASSIGN.Id_plaza,
 		                    LANE_ASSIGN.Id_lane,
		                    TO_CHAR(LANE_ASSIGN.MSG_DHM,'MM/DD/YY HH24:MI:SS') AS FECHA_INICIO,
 		                    LANE_ASSIGN.SHIFT_NUMBER,
 		                    LANE_ASSIGN.OPERATION_ID,
		                    TO_CHAR(LANE_ASSIGN.ASSIGN_DHM,'MM/DD/YY') AS FECHA,
		                    LTRIM(TO_CHAR(LANE_ASSIGN.JOB_NUMBER,'09')) AS EMPLEADO,
		                    LANE_ASSIGN.STAFF_NUMBER,
		                    LANE_ASSIGN.IN_CHARGE_SHIFT_NUMBER
                            FROM 	LANE_ASSIGN
                            WHERE	 SHIFT_NUMBER = " + TurnoP + "" +
                            "AND LANE_ASSIGN.OPERATION_ID = 'NA'" +
                            "AND((MSG_DHM >= TO_DATE('" + FechaInicio + "', 'MM-DD-YYYY HH24:MI:SS')) AND(MSG_DHM <= TO_DATE('" + FechaFinal + "', 'MM-DD-YYYY HH24:MI:SS')))" +
                            "ORDER BY LANE_ASSIGN.Id_PLAZA," +
                            "LANE_ASSIGN.Id_LANE," +
                            "LANE_ASSIGN.MSG_DHM ";

            // Se llaman a todos los carriles con NA
            Connection.Open();
            Cmd.CommandText = Query.ToString();
            Cmd.Connection = Connection;
            OracleDataReader DataReader = Cmd.ExecuteReader();
            while (DataReader.Read())
            {
                Carril = new Carril();
                Carril.LANE = DataReader["ID_LANE"].ToString();
                Carril.FECHA = DataReader["FECHA_INICIO"].ToString();
                Carril.MATRICULE = DataReader["STAFF_NUMBER"].ToString();
                Carriles.Add(Carril);
            }
            Connection.Close();

            // Se verifican que los carriles se encuentren cerrados en la tabla FIN_POSTE
            foreach (Carril item in Carriles)
            {
                string QueryFin_Poste = @"SELECT COUNT(*) FROM FIN_POSTE WHERE DATE_DEBUT_POSTE = TO_DATE('" + item.FECHA + "', 'MM/DD/YY HH24:MI:SS') AND VOIE = '" + item.LANE + "' AND MATRICULE = '" + item.MATRICULE + "'";
                Connection.Open();
                Cmd.CommandText = QueryFin_Poste;
                Cmd.Connection = Connection;
                if (Convert.ToInt32(Cmd.ExecuteScalar()) < 1)
                {
                    Carril = new Carril();
                    Carril.LANE = item.LANE;
                    Carril.FECHA = item.FECHA;
                    Carril.MATRICULE = item.MATRICULE;
                    CarrilesCerrados.Add(Carril);
                    BanValidaciones = false;
                }
                Connection.Close();
            }

            foreach (Carril value in CarrilesCerrados)
            {
                Message += value.LANE + ", ";
            }

            rpt = BanValidaciones == true ? "OK" : "STOP";

            return rpt;
        }

        /// <summary>
        /// Validar bolsas
        /// </summary>
        /// <param name="FechaInicioD"></param>
        /// <param name="FechaSelect"></param>
        /// <param name="TempTurno"></param>
        /// <returns></returns>
        public string ValidarBolsas(DateTime FechaInicioD, DateTime FechaSelect, string TempTurno, string Conexion)
        {
            OracleCommand Cmd = new OracleCommand();
            OracleConnection Connection = new OracleConnection(Conexion);
            //Connection.ConnectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            string rpt = string.Empty;
            string FechaInicio = string.Empty;
            string FechaFinal = string.Empty;
            string TurnoP = string.Empty;
            BanValidaciones = true;

            switch (TempTurno.Substring(0, 2))
            {
                case "06":
                    TurnoP = "2";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 06:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 13:59:59";
                    break;
                case "14":
                    TurnoP = "3";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 14:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 21:59:59";
                    break;
                case "22":
                    TurnoP = "1";
                    FechaInicio = FechaInicioD.AddDays(-1).ToString("MM/dd/yyyy") + " 22:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 05:59:59";
                    break;
            }


            // Verifica que todos los carriles cerrados tengan bolsa
            string Query = @"SELECT TO_CHAR(C.DATE_FIN_POSTE,'yyyy-mm-dd') AS FECHA, " +
                            "C.MATRICULE AS cajero, " +
                            "C.VOIE AS Carril, " +
                            "C.NUMERO_POSTE AS Corte, " +
                            "TO_CHAR(C.DATE_DEBUT_POSTE,'HH24:mi:SS') AS Inicio_Turno, " +
                            "TO_CHAR(C.DATE_FIN_POSTE,'HH24:mi:SS') AS Fin_Turno, " +
                            "'Entrega no realizada de bolsa '||C.VOIE||' Inicio '||TO_CHAR(C.DATE_DEBUT_POSTE,'HH24:mi:SS')||',Fin '||TO_CHAR(C.DATE_FIN_POSTE,'HH24:mi:SS')||' '||A.MATRICULE||'/'|| A.NOM AS Aviso " +
                            "FROM FIN_POSTE C " +
                            "LEFT JOIN TABLE_PERSONNEL  A ON C.Matricule = A.Matricule " +
                            "WHERE C.DATE_DEBUT_POSTE " +
                            "BETWEEN to_date('" + FechaInicio + "' ,'mm-dd-yyyy HH24:mi:SS') " +
                            "AND to_date('" + FechaFinal + "' ,'mm-dd-yyyy HH24:mi:SS') " +
                            "AND SAC IS NULL AND FIN_POSTE_CPT22 = " + TurnoP + "AND C.ID_MODE_VOIE in (1,7)";
            Connection.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Connection;
            OracleDataReader DataReader = Cmd.ExecuteReader();
            while (DataReader.Read())
            {
                BanValidaciones = false;
                Message += DataReader["Aviso"].ToString();
            }
            Connection.Close();

            rpt = BanValidaciones == true ? "OK" : "STOP";

            return rpt;
        }

        /// <summary>
        /// Validar Comentarios
        /// </summary>
        /// <param name="FechaInicioD"></param>
        /// <param name="FechaSelect"></param>
        /// <param name="TempTurno"></param>
        /// <returns></returns>
        //public List<filas> ValidarComentarios(DateTime FechaInicioD, DateTime FechaSelect, string TempTurno)
        public string ValidarComentarios(DateTime FechaInicioD, DateTime FechaSelect, string TempTurno, string Conexion)
        {
            OracleCommand Cmd = new OracleCommand();
            OracleConnection Connection = new OracleConnection(Conexion);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //Connection.ConnectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            string rpt = string.Empty;
            string FechaInicio = string.Empty;
            string FechaFinal = string.Empty;
            string TurnoP = string.Empty;
            BanValidaciones = true;

            switch (TempTurno.Substring(0, 2))
            {
                case "06":
                    TurnoP = "2";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 06:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 13:59:59";
                    break;
                case "14":
                    TurnoP = "3";
                    FechaInicio = FechaInicioD.ToString("MM/dd/yyyy") + " 14:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 21:59:59";
                    break;
                case "22":
                    TurnoP = "1";
                    FechaInicio = FechaInicioD.AddDays(-1).ToString("MM/dd/yyyy") + " 22:00:00";
                    FechaFinal = FechaSelect.ToString("MM/dd/yyyy") + " 05:59:59";
                    break;
            }

            // Valida que se hayan capturado los  comentarios en la entrega de Bolsa
            // SE MODIFICIO DATE_FIN_POSTE POR C.DATE_DEBUT_POSTE [RODRIGO]




            string Query = @"SELECT " +
                            "C.COMMENTAIRE AS COMENTARIOS, " +
                            "C.SAC AS BOLSA, " +
                            "C.OPERATING_SHIFT AS TURNO, " +
                            "C.DATE_REDDITION AS FECHA, " +
                            "C.RED_TXT1, " +
                            "''||C.RED_TXT1||' bolsa '||TO_CHAR(C.SAC)||' '||A.MATRICULE||'/'|| A.NOM ||'                          ' AS Aviso " +
                            "FROM REDDITION  C " +
                            "LEFT JOIN TABLE_PERSONNEL  A ON C.Matricule = A.Matricule " +
                            "WHERE DATE_REDDITION " +
                            "BETWEEN to_date('" + FechaInicio + "' ,'mm-dd-yyyy HH24:mi:SS') " +
                            "AND to_date('" + FechaFinal + "' ,'mm-dd-yyyy HH24:mi:SS') " +
                            " AND COMMENTAIRE IS NULL AND C.OPERATING_SHIFT  = " + TurnoP;



            Connection.Open();
            Cmd.CommandText = Query;
            Cmd.Connection = Connection;
            OracleDataAdapter myAdapter = new OracleDataAdapter(Cmd);
            OracleDataReader DataReader = Cmd.ExecuteReader();

            while (DataReader.Read())
            {
                BanValidaciones = false;
                myAdapter.Fill(dt);
                Message += DataReader["Aviso"].ToString();
                // break;


            }


            //List<filas> filass = new List<filas>();
            foreach (DataRow indi in dt.Rows)
            {
                filas datos = new filas();
                datos.bolsa = indi["BOLSA"].ToString();
                datos.red = indi["RED_TXT1"].ToString();
                datos.turno = indi["TURNO"].ToString();
                listass.Add(datos);
                break;
            }


            ControlesExportar model = new ControlesExportar();
            model.Listacomentarios = listass;
            Connection.Close();

            rpt = BanValidaciones == true ? "OK" : "STOP";
            return rpt;

            //return listass;
        }


        //public string Insertar_Comentarios(DateTime FechaInicioD, DateTime FechaSelect, string TempTurno, string Comentario)
        public string Isertar_Comentarios(List<filas> listass, List<string> comentario, string Conexion)
        {

            string rpt = string.Empty;

            for (int i = 0; i < comentario.Count; i++)
            {



                string Query = "UPDATE REDDITION SET COMMENTAIRE = '" + comentario[i] +
                                "' WHERE SAC = '" + listass[i].bolsa +
                                "' AND OPERATING_SHIFT = '" + listass[i].turno +
                                "' ";


                //string ConnectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

                try
                {
                    using (OracleConnection Connection = new OracleConnection(Conexion))

                    {
                        OracleCommand command = new OracleCommand(Query, Connection);
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }

            }

            BanValidaciones = true;
            Message = "MENSAJE INSERTADO";
            rpt = BanValidaciones == true ? "OK" : "STOP";

            return rpt;
        }

        public string Valida_Turno(int valida, DateTime dia)
        {
            string rpt = string.Empty;

            DateTime Dia_Actual = DateTime.Today;
            DateTime Hora_Actual = DateTime.Now;
            DateTime _hora = Convert.ToDateTime("22:00:00").AddDays(-1);
            DateTime hora_ = Convert.ToDateTime("05:59:59");
            DateTime _hora1 = Convert.ToDateTime("06:00:00");
            DateTime hora1_ = Convert.ToDateTime("13:59:59");
            DateTime _hora2 = Convert.ToDateTime("14:00:00");
            DateTime hora2_ = Convert.ToDateTime("21:59:59");




            if (dia < Dia_Actual)
            {
                rpt = "Ok";
                return (rpt);
            }
            else
            {

                switch (valida)
                {
                    case 1:
                        {

                            if (Valida1(Hora_Actual, _hora, hora_) == true)
                            {
                                rpt = "Ok";


                            }
                            else
                            {
                                rpt = "STOP";
                            }

                            return (rpt);

                        }
                    case 2:
                        {

                            if (Valida2(Hora_Actual, _hora1, hora1_) == true)
                            {
                                rpt = "Ok";
                            }
                            else
                            {
                                rpt = "STOP";
                            }

                            return (rpt);

                        }
                    case 3:
                        {


                            if (Valida3(Hora_Actual, _hora2, hora2_) == true)
                            {
                                rpt = "Ok";
                            }
                            else
                            {
                                rpt = "STOP";
                            }

                            return (rpt);

                        }

                }

            }
            return ("OK");
        }


        public bool Valida1(DateTime hora_actual, DateTime hora1, DateTime hora2)
        {

            if (hora_actual > hora1 && hora_actual > hora2)
            {
                BanValidaciones = true;
            }
            else
            {
                BanValidaciones = false;
            }
            return (BanValidaciones);
        }
        public bool Valida2(DateTime hora_actual, DateTime hora1, DateTime hora2)
        {

            if (hora_actual > hora1 && hora_actual > hora2)
            {
                BanValidaciones2 = true;
            }
            else
            {
                BanValidaciones2 = false;
            }
            return (BanValidaciones2);
        }
        public bool Valida3(DateTime hora_actual, DateTime hora1, DateTime hora2)
        {

            if (hora_actual > hora1 && hora_actual > hora2)
            {
                BanValidaciones3 = true;
            }
            else
            {
                BanValidaciones3 = false;
            }
            return (BanValidaciones3);
        }
    }


    public class Carril
    {
        public string LANE { get; set; }
        public string FECHA { get; set; }
        public string MATRICULE { get; set; }
    }

    public class ControlesExportar
    {
        public List<SelectListItem> ListDelegaciones { get; set; }


        public string DelegacionesId { get; set; }

        public List<SelectListItem> ListPlazaCobro { get; set; }

        public string PlazaCobroId { get; set; }

        public List<SelectListItem> ListTurno { get; set; }

        public string TurnoId { get; set; }

        public string EncargadoTurno { get; set; }

        public DateTime FechaFin { get; set; }

        public DateTime FechaInicio { get; set; }

        public List<HttpPostedFileBase> files { get; set; }

        public List<string> Comentario { get; set; }

        public bool Valida_Pop { get; set; }

        public List<filas> Listacomentarios { get; set; }

    }

    public class filas
    {
        public string bolsa { get; set; }
        public string red { get; set; }
        public string turno { get; set; }
    }
}
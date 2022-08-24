using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using ReportesWeb1_2.ModelsReportes;
using ReportesWeb1_2.ModelsSQLServer;
using ReportesWeb1_2.Services;
using ReportesWeb1_2.ServicesReportes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReportesWeb1_2.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private ProsisSQLServerModel db = new ProsisSQLServerModel();
        private MetodosGlbRepository MtGlb = new MetodosGlbRepository();
        private Validation validaciones = new Validation();
        private CajeroReceptorRepository CaReRepository = new CajeroReceptorRepository();
        private TurnoCarrilesRepository TuCaRepository = new TurnoCarrilesRepository();
        private DiaCasetaRepository DiCaRepository = new DiaCasetaRepository();

        static string NameConnectionString = string.Empty;
        static string DelegacionBag = string.Empty;
        static string PlazaBag = string.Empty;
        static string IdPlaza = string.Empty;
        static string IdDelegacion = string.Empty;
        static string NomPlaza = string.Empty;

        private async Task UserPlaza()
        {
            await Task.Run(() =>
            {
                var userMatricula = User.Identity.Name.Substring(0, User.Identity.Name.LastIndexOf('@'));

                var Result = db.Type_Delegacion
                                .GroupJoin(
                                    db.Type_Plaza,
                                    del => del.Id_Delegacion,
                                    pla => pla.Delegacion_Id,
                                    (del, pla) => new { del, pla })
                                .SelectMany(del => del.pla.DefaultIfEmpty(), (del, pla) => new { del.del, pla })
                                .Join(
                                    db.Type_Operadores,
                                    pla => pla.pla.Id_Plaza,
                                    ope => ope.Plaza_Id,
                                    (pla, ope) => new { pla, ope })
                                .Where(x => x.ope.Num_Capufe == userMatricula).FirstOrDefault();

                NameConnectionString = "Oracle" + Result.pla.pla.Num_Plaza;
                DelegacionBag = Result.pla.del.Nom_Delegacion;
                IdDelegacion = Result.pla.del.Num_Delegacion;
                PlazaBag = Result.pla.pla.Num_Plaza + " " + Result.pla.pla.Nom_Plaza;
                IdPlaza = Result.pla.pla.Num_Plaza;
                NomPlaza = Result.pla.pla.Nom_Plaza;
            });
        }

        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Cajero - Receptor Form
        [HttpGet]
        public async Task<ActionResult> CajeroReceptorIndex()
        {
            await UserPlaza();
            var model = new CajeroReceptorModel()
            {
                ListBolsas = new List<Bolsas>(),
            };

            ViewBag.Delegacion = DelegacionBag;

            string aux = "";

            if (PlazaBag.Contains("08"))
            {
                aux = "001" + " " + NomPlaza;
                ViewBag.Plaza = aux;
            }
            else
            {
                ViewBag.Plaza = PlazaBag;
            }

            return View(model);
        }

        //POST: Cajero - Receptor Form
        [HttpPost]
        public ActionResult CajeroReceptorIndex(CajeroReceptorModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string errorCentavos = "";
                    string errorComentarios = "";
                    string errorDiferenciaSegundos = "";
                    bool errorValidacion = true;
                    var table = new DataTable("FIN_POSTE");
                    string Query = $"SELECT * FROM FIN_POSTE WHERE MATRICULE = '{model.NumCajeroReceptor}'"; //Query para traer datos si existe el cajero
                    bool validation;

                    OracleCommand Command = new OracleCommand(Query, MtGlb.GetConnectionOracle(NameConnectionString));

                    using (OracleDataAdapter adapter = new OracleDataAdapter(Command))
                    {
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            validation = true;
                        }
                        else
                        {
                            validation = false;
                        }
                    }

                    if (validation == true)
                    {
                        var Delegaciones = new JavaScriptSerializer().Serialize(GetDelegaciones().Data);
                        model.ListDelegaciones = JsonConvert.DeserializeObject<List<SelectListItem>>(Delegaciones);

                        var Plazas = new JavaScriptSerializer().Serialize(GetPlazas().Data);
                        model.ListPlazas = JsonConvert.DeserializeObject<List<SelectListItem>>(Plazas);

                        var Turnos = new JavaScriptSerializer().Serialize(GetTurnos().Data);
                        model.ListTurnos = JsonConvert.DeserializeObject<List<SelectListItem>>(Turnos);

                        var Administradores = new JavaScriptSerializer().Serialize(GetAdministradores().Data);
                        model.ListAdministradores = JsonConvert.DeserializeObject<List<SelectListItem>>(Administradores);

                        var Delegacion = model.ListDelegaciones.Find(x => x.Value == IdDelegacion);
                        var Plaza = model.ListPlazas.Find(x => x.Value == IdPlaza);
                        var Turno = model.ListTurnos.Find(x => x.Value == model.IdTurno);
                        var Administrador = model.ListAdministradores.Find(x => x.Value == model.IdAdministrador);
                        string admin_num = "";
                        string Matricule_Cajero = Administrador.Value;
                        var Query_Cajero = db.Type_Operadores.Where(x => x.Num_Gea == Matricule_Cajero).FirstOrDefault();

                        if (Query_Cajero != null)
                            admin_num = Query_Cajero.Num_Capufe;
                        else
                            admin_num = Administrador.Value;

                        List<Bolsas> listaBolsas = new List<Bolsas>();
                        List<Bolsas> lista = new List<Bolsas>();

                        listaBolsas = CaReRepository._PartialViewBolsas(model.Fecha, Plaza.Value, Turno.Text, model.NumCajeroReceptor, Delegacion.Text, admin_num + "    " + Administrador.Text, NameConnectionString);

                        if (listaBolsas.Count != 0)
                        {
                            foreach (var item in listaBolsas)
                            {
                                //Query para obtener el saldo total de la bolsa
                                string StrQuerys = "SELECT MATRICULE,SAC AS Expr1, NVL(NB_POSTE, 0) + NVL(NB_POSTE_POS, 0) AS Expr2, NVL(RED_RCT_MONNAIE1, 0) AS Expr3, " +
                                    "NVL(RED_RCT_CHQ, 0) AS Expr4, NVL(RED_NB_CHQ, 0) AS Expr5, NVL(RED_RCT_DEVISE, 0) AS Expr6, NVL(RED_RCT_MONNAIE3, 0) AS Expr7, " +
                                    "NVL(RED_RCT_MONNAIE4, 0) AS Expr8, NVL(RED_CPT1, 0) AS Expr9, NVL(RED_RCT_MONNAIE1, 0) + NVL(RED_RCT_MONNAIE2, 0) + NVL(RED_RCT_MONNAIE3, 0) " +
                                    "+ NVL(RED_RCT_MONNAIE4, 0) + NVL(RED_RCT_CHQ, 0) + NVL(RED_RCT_DEVISE, 0) AS Expr10, NVL(POSTE_RCT_MONNAIE1, 0) + NVL(POSTE_RCT_MONNAIE2, 0) " +
                                    "+ NVL(POSTE_RCT_MONNAIE3, 0) + NVL(POSTE_RCT_MONNAIE4, 0) + NVL(POSTE_RCT_DEVISE, 0) + NVL(POSTE_RCT_CHQ, 0) + NVL(POSTE_POS_RCT_MONNAIE1, " +
                                    "0) + NVL(POSTE_POS_RMB_MONNAIE1, 0) + NVL(POSTE_POS_RCT_MONNAIE2, 0) + NVL(POSTE_POS_RCT_MONNAIE3, 0) + NVL(POSTE_POS_RCT_MONNAIE4, 0) " +
                                    "+ NVL(POSTE_POS_RCT_DEVISE, 0) + NVL(POSTE_POS_RCT_CHQ, 0) AS Expr11, NVL(RED_RCT_MONNAIE1, 0) + NVL(RED_RCT_MONNAIE2, 0) " +
                                    "+ NVL(RED_RCT_MONNAIE3, 0) + NVL(RED_RCT_MONNAIE4, 0) + NVL(RED_RCT_DEVISE, 0) + NVL(RED_RCT_CHQ, 0) - NVL(POSTE_RCT_MONNAIE1, 0) " +
                                    "- NVL(POSTE_RCT_MONNAIE2, 0) - NVL(POSTE_RCT_MONNAIE3, 0) - NVL(POSTE_RCT_MONNAIE4, 0) - NVL(POSTE_RCT_DEVISE, 0) - NVL(POSTE_RCT_CHQ, 0) " +
                                    "- NVL(POSTE_POS_RCT_MONNAIE1, 0) - NVL(POSTE_POS_RMB_MONNAIE1, 0) - NVL(POSTE_POS_RCT_MONNAIE2, 0) - NVL(POSTE_POS_RCT_MONNAIE3, 0) " +
                                    "- NVL(POSTE_POS_RCT_MONNAIE4, 0) - NVL(POSTE_POS_RCT_DEVISE, 0) - NVL(POSTE_POS_RCT_CHQ, 0) AS Expr12, NVL(RED_RCT_MONNAIE1, 0) " +
                                    "- NVL(POSTE_RCT_MONNAIE1, 0) - NVL(POSTE_POS_RMB_MONNAIE1, 0) - NVL(POSTE_POS_RCT_MONNAIE1, 0) AS Expr13, NVL(RED_RCT_CHQ, 0) " +
                                    "- NVL(POSTE_RCT_CHQ, 0) - NVL(POSTE_POS_RCT_CHQ, 0) AS Expr14, NVL(RED_RCT_DEVISE, 0) - NVL(POSTE_RCT_DEVISE, 0) - NVL(POSTE_POS_RCT_DEVISE, 0) " +
                                    "AS Expr15, NVL(RED_CPT24, 0) AS Expr16, NVL(RED_CPT25, 0) AS Expr17, NVL(RED_JETON, 0) - NVL(POSTE_JETON, 0) AS Expr18, NVL(RED_RDD, 0) " +
                                    "- NVL(POSTE_RDD, 0) AS Expr19, NVL(RED_GRATUIT, 0) + NVL(RED_CPT2, 0) - NVL(POSTE_GRATUIT, 0) AS Expr20, MATRICULE_COMMENTAIRE, COMMENTAIRE, " +
                                    "0 AS Expr21, TO_CHAR(DATE_REDDITION, 'YYYYMMDDHH24MISS') AS Expr22, ID_SITE, RED_CPT21, NB_POSTE, ETAT_REDDITION, " +
                                    "NVL(Red_Rct_Monnaie1,0)	+ NVL(Red_Rct_Devise,0)	+ NVL(Red_Rct_Chq,0) + NVL(Red_cpt21,0)	- NVL(Poste_Rct_Monnaie1,0) - NVL(Poste_Rct_Devise,0) - NVL(Poste_Rct_Chq,0) AS Expr23, RED_CPT22  " +
                                    "FROM REDDITION " +
                                    "WHERE  (ID_SITE = '" + Plaza.Value + "') AND (MATRICULE = '" + model.NumCajeroReceptor + "') AND (SAC = '" + item.Bolsa + "')";

                                if (MtGlb.QueryDataSet1(StrQuerys, "REDDITION"))
                                {
                                    var saldo = MtGlb.oDataRow1["Expr3"].ToString();

                                    if (Convert.ToString(saldo).Contains(".")) //Se valida que el saldo de la bolsa no contenga punto decimal, ya que tener punto decimal dice que tiene centavos
                                    {
                                        errorValidacion = false;
                                        errorCentavos = errorCentavos + $"La bolsa {item.Bolsa} fue declarada con centavos, favor de volverla a declarla sin centavos.";
                                    }

                                    if (!DBNull.Value.Equals(MtGlb.oDataRow1["COMMENTAIRE"])) //Se valida que la bolsa contenga comentarios 
                                    {
                                        if (errorValidacion == true)
                                        {
                                            DateTime inicio = DateTime.ParseExact(item.Inicio, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                            string StrQuerys2 = "SELECT	LANE_ASSIGN.Id_plaza,LANE_ASSIGN.Id_lane,TO_CHAR(LANE_ASSIGN.MSG_DHM,'MM/DD/YY HH24:MI:SS'),LANE_ASSIGN.SHIFT_NUMBER,LANE_ASSIGN.OPERATION_ID, " +
                                                   "LANE_ASSIGN.DELEGATION, TO_CHAR(LANE_ASSIGN.ASSIGN_DHM,'MM/DD/YY'),LTRIM(TO_CHAR(LANE_ASSIGN.JOB_NUMBER,'09')),	LANE_ASSIGN.STAFF_NUMBER,LANE_ASSIGN.IN_CHARGE_SHIFT_NUMBER " +
                                                   "FROM 	LANE_ASSIGN, SITE_GARE " +
                                                   "WHERE	LANE_ASSIGN.id_NETWORK = SITE_GARE.id_Reseau " +
                                                   "AND LANE_ASSIGN.id_plaza = SITE_GARE.id_Gare " +
                                                   "AND SITE_GARE.id_reseau = '01' " +
                                                   "AND	SITE_GARE.id_Site ='" + Plaza.Value + "' " +
                                                   "AND LANE_ASSIGN.Id_lane = '" + item.Carril + "'" +
                                                   "AND (MSG_DHM >= TO_DATE('" + inicio.ToString("yyyyMMddHHmmss") + "', 'YYYYMMDDHH24MISS')) AND " +
                                                   "(MSG_DHM <= TO_DATE('" + inicio.ToString("yyyyMMddHHmmss") + "', 'YYYYMMDDHH24MISS')) " +
                                                   "ORDER BY LANE_ASSIGN.Id_PLAZA, LANE_ASSIGN.Id_LANE, LANE_ASSIGN.MSG_DHM";

                                            if (MtGlb.QueryDataSet2(StrQuerys2, "Asig_Carril"))
                                            {
                                                //Despues de validar que la bolsa generada tenga comentarios y su saldo no sea con centavos, se guarda la bolsa
                                                lista.Add(new Bolsas
                                                {
                                                    Id = item.Id,
                                                    Inicio = item.Inicio,
                                                    Fin = item.Fin,
                                                    Carril = item.Carril,
                                                    Bolsa = item.Bolsa
                                                });
                                            }
                                            else
                                            {
                                                errorDiferenciaSegundos = errorDiferenciaSegundos + $"No se pudo generar la bolsa {item.Bolsa}, favor de notificarle a sistemas";
                                                string path = @"C:\Log\CajeroReceptor.txt";
                                                StreamWriter sw = new StreamWriter(path, true);

                                                sw.WriteLine($"La bolsa {item.Bolsa}, tiene una diferencia de segundos, favor de arreglar la fecha");

                                                sw.Flush();
                                                sw.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errorComentarios = errorComentarios + $"La bolsa {item.Bolsa} no tiene observaciones o comentarios, favor de ingresarlos.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Nulo = "rojo";
                        }

                        ViewBag.ErrorCentavos = errorCentavos;
                        ViewBag.ErrorComentarios = errorComentarios;
                        ViewBag.errorDiferenciaSegundos = errorDiferenciaSegundos;

                        model.ListBolsas = lista;

                        return PartialView("_ListaBolsasPartial", model);
                    }
                    else
                    {
                        ViewBag.Error = $"El cajero {model.NumCajeroReceptor} no existe, favor de ingresar un cajero que exista.";

                        return PartialView("_ListaBolsasPartial", model);
                    }
                }
                return View(model);

            }
            catch (Exception ex)
            {
                string path = @"C:\Log\CajeroReceptor.txt";
                StreamWriter sw = new StreamWriter(path, true);

                sw.WriteLine(ex.Message);

                sw.Flush();
                sw.Close();
                return null;
            }
        }


        // GET: Report Book CajeroReceptor
        [HttpGet]
        public ActionResult ReportCajeroReceptorView(int? IdRow)
        {
            try
            {
                if (IdRow == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                CajeroReceptorModel model = new CajeroReceptorModel
                {
                    PreCRViewModel = CaReRepository.GenerarPreliquidacion_Cajero_Receptor(IdRow),
                    ComCRViewModel = CaReRepository.GenerarComparativo_Cajero_Receptor()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                string path = @"C:\Log\ErrorSource.txt";

                StreamWriter sw = new StreamWriter(path, true);

                sw.WriteLine(ex.Message);

                sw.Flush();
                sw.Close();
                return null;
            }
        }

        // GET: Turno/Carriles Form
        [HttpGet]
        public async Task<ActionResult> TurnoCarrilesIndex()
        {
            await UserPlaza();
            var model = new TurnoCarrilesModel();

            ViewBag.Delegacion = DelegacionBag;

            string aux = "";
            if (PlazaBag.Contains("08"))
            {
                aux = "001" + " " + NomPlaza;
                ViewBag.Plaza = aux;
            }
            else
            {
                ViewBag.Plaza = PlazaBag;
            }

            if (TempData.ContainsKey("Carril"))
                ViewBag.Error = TempData["Carril"].ToString();
            else if (TempData.ContainsKey("Bolsa"))
                ViewBag.Error = TempData["Bolsa"].ToString();
            else if (TempData.ContainsKey("Comenta"))
                ViewBag.Error = TempData["Comenta"].ToString();
            else
                ViewBag.Error = null;

            return View(model);
        }

        // GET: Report Book TurnoCarriles
        [HttpGet]
        public ActionResult ReportTurnoCarrilesView()
        {
            return HttpNotFound();
        }

        //POST: Turno/Carriles Form
        [HttpPost]
        public ActionResult ReportTurnoCarrilesView(TurnoCarrilesModel model)
        {
            try
            {


                //DataSet comTCViewModel = new DataSet();
                if (ModelState.IsValid)
                {

                    var Delegaciones = new JavaScriptSerializer().Serialize(GetDelegaciones().Data);
                    model.ListDelegaciones = JsonConvert.DeserializeObject<List<SelectListItem>>(Delegaciones);

                    var Plazas = new JavaScriptSerializer().Serialize(GetPlazas().Data);
                    model.ListPlazas = JsonConvert.DeserializeObject<List<SelectListItem>>(Plazas);

                    var Turnos = new JavaScriptSerializer().Serialize(GetTurnos().Data);
                    model.ListTurnos = JsonConvert.DeserializeObject<List<SelectListItem>>(Turnos);

                    var Administradores = new JavaScriptSerializer().Serialize(GetAdministradores().Data);
                    model.ListAdministradores = JsonConvert.DeserializeObject<List<SelectListItem>>(Administradores);

                    var EncargadosTurno = new JavaScriptSerializer().Serialize(GetEncargadosTurno().Data);
                    model.ListEncargadosTurno = JsonConvert.DeserializeObject<List<SelectListItem>>(EncargadosTurno);

                    var Delegacion = model.ListDelegaciones.Find(x => x.Value == IdDelegacion);
                    var Plaza = model.ListPlazas.Find(x => x.Value == IdPlaza);
                    var Turno = model.ListTurnos.Find(x => x.Value == model.IdTurno);
                    var Administrador = model.ListAdministradores.Find(x => x.Value == model.IdAdministrador);
                    var EncargadoTurno = model.ListEncargadosTurno.Find(x => x.Value == model.IdEncargadoTurno);
                    string ConexionDB = string.Empty;

                    ConexionDB = ConfigurationManager.ConnectionStrings[NameConnectionString].ConnectionString;

                    if (validaciones.ValidarCarrilesCerrados(model.Fecha, model.Fecha, Turno.Text, ConexionDB) == "STOP")
                    {
                        TempData["Carril"] = "Existen carriles abiertos: " + validaciones.Message;

                        return RedirectToAction("TurnoCarrilesIndex", model);
                    }
                    else if (validaciones.ValidarBolsas(model.Fecha, model.Fecha, Turno.Text, ConexionDB) == "STOP")
                    {
                        TempData["Bolsa"] = "Existen bolsas sin declarar: " + validaciones.Message;

                        return RedirectToAction("TurnoCarrilesIndex", model);
                    }
                    else if (validaciones.ValidarComentarios(model.Fecha, model.Fecha, Turno.Text, ConexionDB) == "STOP")
                    {
                        TempData["Comenta"] = "Falta ingresar comentarios: " + validaciones.Message;

                        return RedirectToAction("TurnoCarrilesIndex", model);
                    }

                    string encargado_num = "";
                    string admin_num = "";
                    string Matricule_Cajero = EncargadoTurno.Value;
                    var Query_Cajero = db.Type_Operadores.Where(x => x.Num_Gea == Matricule_Cajero).FirstOrDefault();

                    if (Query_Cajero != null)
                        encargado_num = Query_Cajero.Num_Capufe;
                    else
                        encargado_num = EncargadoTurno.Value;

                    Matricule_Cajero = Administrador.Value;
                    Query_Cajero = db.Type_Operadores.Where(x => x.Num_Gea == Matricule_Cajero).FirstOrDefault();

                    if (Query_Cajero != null)
                        admin_num = Query_Cajero.Num_Capufe;
                    else
                        admin_num = Administrador.Value;

                    var preTCViewModel = TuCaRepository.GenerarPreliquidacion_Turno_Carriles(model.Fecha, Plaza.Value, Turno.Text, encargado_num + "    " + EncargadoTurno.Text, Delegacion.Text, admin_num + "    " + Administrador.Text, model.Observaciones, NameConnectionString);
                    var comTCViewModel = TuCaRepository.GenerarComparativo_Turno_Carriles();

                    model.PreTCViewModel = preTCViewModel;
                    model.ComTCViewModel = comTCViewModel;

                    //return View("Pruebas", comTCViewModel);
                    return View(model);
                }

                return RedirectToAction("TurnoCarrilesIndex");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // GET: Dia/Caseta Form
        [HttpGet]
        public async Task<ActionResult> DiaCasetaIndex()
        {
            await UserPlaza();
            var model = new DiaCasetaModel();
            ViewBag.Delegacion = DelegacionBag;

            string aux = "";
            if (PlazaBag.Contains("08"))
            {
                aux = "001" + " " + NomPlaza;
                ViewBag.Plaza = aux;
            }
            else
            {
                ViewBag.Plaza = PlazaBag;
            }


            return View(model);
        }

        // GET: Report Book DiaCaseta
        [HttpGet]
        public ActionResult ReportDiaCasetaView()
        {
            return HttpNotFound();
        }

        //POST: Dia/Caseta Form
        [HttpPost]
        public ActionResult ReportDiaCasetaView(DiaCasetaModel model)
        {
            //DataSet comTCViewModel = new DataSet();
            if (ModelState.IsValid)
            {
                var Delegaciones = new JavaScriptSerializer().Serialize(GetDelegaciones().Data);
                model.ListDelegaciones = JsonConvert.DeserializeObject<List<SelectListItem>>(Delegaciones);

                var Plazas = new JavaScriptSerializer().Serialize(GetPlazas().Data);
                model.ListPlazas = JsonConvert.DeserializeObject<List<SelectListItem>>(Plazas);

                var Administradores = new JavaScriptSerializer().Serialize(GetAdministradores().Data);
                model.ListAdministradores = JsonConvert.DeserializeObject<List<SelectListItem>>(Administradores);

                var EncargadosTurno = new JavaScriptSerializer().Serialize(GetEncargadosTurno().Data);
                model.ListEncargadosTurno = JsonConvert.DeserializeObject<List<SelectListItem>>(EncargadosTurno);

                var Delegacion = model.ListDelegaciones.Find(x => x.Value == IdDelegacion);
                var Plaza = model.ListPlazas.Find(x => x.Value == IdPlaza);
                var Administrador = model.ListAdministradores.Find(x => x.Value == model.IdAdministrador);
                var EncargadoTurno = model.ListEncargadosTurno.Find(x => x.Value == model.IdEncargadoTurno);

                string encargado_num = "";
                string admin_num = "";
                string Matricule_Cajero = EncargadoTurno.Value;
                var Query_Cajero = db.Type_Operadores.Where(x => x.Num_Gea == Matricule_Cajero).FirstOrDefault();

                if (Query_Cajero != null)
                    encargado_num = Query_Cajero.Num_Capufe;
                else
                    encargado_num = EncargadoTurno.Value;

                Matricule_Cajero = Administrador.Value;
                Query_Cajero = db.Type_Operadores.Where(x => x.Num_Gea == Matricule_Cajero).FirstOrDefault();

                if (Query_Cajero != null)
                    admin_num = Query_Cajero.Num_Capufe;
                else
                    admin_num = Administrador.Value;

                var preDCViewModel = DiCaRepository.GenerarPreliquidacion_Dia_Caseta(model.Fecha, Plaza.Value, encargado_num + "    " + EncargadoTurno.Text, Delegacion.Text, admin_num + "    " + Administrador.Text, model.Observaciones, NameConnectionString);
                var comDCViewModel = DiCaRepository.GenerarComparativo_Dia_Caseta();

                model.PreDCViewModel = preDCViewModel;
                model.ComDCViewModel = comDCViewModel;

                return View(model);
            }

            return RedirectToAction("DiaCasetaIndex");
        }

        #region GetJsonParaDropDownList

        // GET: Delegaciones en formato Json para cargarlos con ajax
        [HttpGet]
        public JsonResult GetDelegaciones()
        {
            var Items = new List<SelectListItem>();

            var Query = db.Type_Delegacion.ToList();

            foreach (var item in Query)
            {
                Items.Add(new SelectListItem
                {
                    Value = item.Num_Delegacion,
                    Text = item.Nom_Delegacion
                });
            }

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        // GET: Plazas de cobro en formato Json para cargarlos con ajax
        [HttpGet]
        public JsonResult GetPlazas()
        {
            var Items = new List<SelectListItem>();

            var Query = db.Type_Plaza.ToList();

            foreach (var item in Query)
            {
                Items.Add(new SelectListItem
                {
                    Value = item.Num_Plaza,
                    Text = item.Num_Plaza + " " + item.Nom_Plaza
                });
            }

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        // GET: Turnos en formato Json para cargarlos con ajax
        [HttpGet]
        public JsonResult GetTurnos()
        {
            var Items = new List<SelectListItem>()
            {

            new SelectListItem
            {
                Text = "22:00 - 06:00",
                Value = "1"
            },

            new SelectListItem
            {
                Text = "06:00 - 14:00",
                Value = "2"
            },

            new SelectListItem
            {
                Text = "14:00 - 22:00",
                Value = "3"
            }
        };

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        // GET: Administradores en formato Json para cargarlos con ajax
        [HttpGet]
        public JsonResult GetAdministradores()
        {
            var Items = new List<SelectListItem>();
            var table = new DataTable("TABLE_PERSONNEL_ADMINISTRADOR");
            string Query = "SELECT MATRICULE, rtrim(NOM)||' '||rtrim(PRENOM) AS NOMBRE FROM TABLE_PERSONNEL WHERE MATRICULE LIKE '1%%%%%' ORDER BY NOM ";

            OracleCommand Command = new OracleCommand(Query, MtGlb.GetConnectionOracle(NameConnectionString));

            using (OracleDataAdapter adapter = new OracleDataAdapter(Command))
            {
                adapter.Fill(table);
                if (table.Rows.Count >= 1)
                {
                    foreach (var item in table.AsEnumerable().ToList())
                    {
                        Items.Add(new SelectListItem
                        {
                            Text = item["NOMBRE"].ToString(),
                            Value = item["MATRICULE"].ToString()
                        });
                    }
                }
            }

            return Json(Items, JsonRequestBehavior.AllowGet);

        }

        // GET: EncargadosTurno en formato Json para cargarlos con ajax
        [HttpGet]
        public JsonResult GetEncargadosTurno()
        {
            var Items = new List<SelectListItem>();
            var table = new DataTable("TABLE_PERSONNEL_ENCARGADOTURNO");
            string Query = "SELECT MATRICULE, rtrim(NOM)||' '||rtrim(PRENOM) AS NOMBRE FROM TABLE_PERSONNEL WHERE MATRICULE LIKE '4%%%%%' ORDER BY NOM ";

            OracleCommand Command = new OracleCommand(Query, MtGlb.GetConnectionOracle(NameConnectionString));

            using (OracleDataAdapter adapter = new OracleDataAdapter(Command))
            {
                adapter.Fill(table);
                if (table.Rows.Count >= 1)
                {
                    foreach (var item in table.AsEnumerable().ToList())
                    {
                        Items.Add(new SelectListItem
                        {
                            Text = item["NOMBRE"].ToString(),
                            Value = item["MATRICULE"].ToString()
                        });
                    }
                }
            }

            return Json(Items, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Pruebas()
        {
            return View();
        }
    }
}
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using ReportesWeb1_2.ModelsReportes;
using ReportesWeb1_2.ModelsSQLServer;
using ReportesWeb1_2.Services;
using ReportesWeb1_2.ServicesReportes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            ViewBag.Plaza = PlazaBag;

            return View(model);
        }

        //POST: Cajero - Receptor Form
        [HttpPost]
        public ActionResult CajeroReceptorIndex(CajeroReceptorModel model)
        {
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
                model.ListBolsas = CaReRepository._PartialViewBolsas(model.Fecha, Plaza.Value, Turno.Text, model.NumCajeroReceptor, Delegacion.Text, admin_num + "    " + Administrador.Text, NameConnectionString);

                return PartialView("_ListaBolsasPartial", model);
            }

            return View(model);
        }

        // GET: Report Book CajeroReceptor
        [HttpGet]
        public ActionResult ReportCajeroReceptorView(int? IdRow)
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

        // GET: Turno/Carriles Form
        [HttpGet]
        public async Task<ActionResult> TurnoCarrilesIndex()
        {
            await UserPlaza();
            var model = new TurnoCarrilesModel();

            ViewBag.Delegacion = DelegacionBag;
            ViewBag.Plaza = PlazaBag;

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

        // GET: Dia/Caseta Form
        [HttpGet]
        public async Task<ActionResult> DiaCasetaIndex()
        {
            await UserPlaza();
            var model = new DiaCasetaModel();
            ViewBag.Delegacion = DelegacionBag;
            ViewBag.Plaza = PlazaBag;


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
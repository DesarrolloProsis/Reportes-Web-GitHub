using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using ReportesWeb1_2.Models;
using ReportesWeb1_2.ModelsReportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ReportesWeb1_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var idUser = User.Identity.GetUserId();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

                //var result = _roleManager.Create(new IdentityRole("Administrador"));

                var userRole = false;
                var _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                //_UserManager.AddToRole(idUser, "Administrador");

                userRole = _UserManager.IsInRole(idUser, "Cajero");

                userRole = _UserManager.IsInRole(idUser, "Administrador");
            }


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ChangePassword()
        {

            ViewBag.Success = null;
            ViewBag.Error = null;

            var usersWithRoles = (from user in db.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in db.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).AsEnumerable().Select(p => new Users()

                                  {
                                      Id = p.UserId,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            return View(usersWithRoles);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(Users model)
        {
            var usersWithRoles = (from user in db.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in db.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).AsEnumerable().Select(p => new Users()

                                  {
                                      Id = p.UserId,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });

            try
            {

                if (!ModelState.IsValid)
                {
                    ViewBag.Error = "Las contraseñas no coinciden.";

                    return View(usersWithRoles);
                }

                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

                ViewBag.Success = null;
                ViewBag.Error = null;

                var userFound = await UserManager.FindByNameAsync(model.Email);
                if (userFound == null)
                {
                    // No revelar que el usuario no existe
                    ViewBag.Error = "¡Ups!, Error inesperado.";

                    return View(usersWithRoles);
                }

                string userId = model.Id;//"<YourLogicAssignsRequestedUserId>";
                string newPassword = model.Password; //"<PasswordAsTypedByUser>";
                string hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);

                ApplicationUser cUser = await store.FindByIdAsync(userId);
                await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                await store.UpdateAsync(cUser);

                ViewBag.Success = "¡Éxito!, contraseña restablecida correctamente del usuario " + model.Email;
                return View(usersWithRoles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "¡Ups!, Error inesperado.";
                return View(usersWithRoles);
            }
        }
    }
}
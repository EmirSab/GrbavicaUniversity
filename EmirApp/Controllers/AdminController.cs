using EmirApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EmirApp.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string userName = form["txtEmail"];
            string email = form["txtEmail"];
            string psw = form["txtPassword"];

            // stvaranja glavnog korisnika
            var user = new ApplicationUser();
            user.UserName = userName;
            user.Email = email;

            var newUser = userManager.Create(user, psw);
            return View();
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(rolename))
            {
                //create a new role
                var role = new IdentityRole(rolename);
                roleManager.Create(role);
            }
            return View("Index");
        }

        //brisanje usera
        public ActionResult DeleteUser()
        {
            ViewBag.Useres = context.Users.Select(u => new SelectListItem { Value = u.UserName, Text = u.UserName }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult DeleteUser(FormCollection form)
        {
            string userName = form["txtEmail"];
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (userName == "harryBalls@gmail.com")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var user = context.Users.First(u => u.UserName == userName);
                userManager.Delete(user);
            }
            return View("Index");
        }

        public ActionResult DeleteRole()
        {
            ViewBag.Roles = context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return View();
        }

        [HttpPost]
        //brisanje role
        public ActionResult DeleteRole(FormCollection form)
        {
            string rolename = form["RoleName"];
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (rolename == "SuperAdmin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var role = context.Roles.First(r => r.Name == rolename);
                roleManager.Delete(role);
            }
            return View("Index");
        }
        public ActionResult AssignRole()
        {
            ViewBag.Roles = context.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            ViewBag.Useres = context.Users.Select(u => new SelectListItem { Value = u.UserName, Text = u.UserName }).ToList() ;
            return View();
        }

        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {
            string username = form["txtUserName"];
            string rolename = form["RoleName"];
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(user.Id, rolename);
            return View("Index");
        }
    }
}
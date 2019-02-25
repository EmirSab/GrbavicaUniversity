using EmirApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmirApp.Startup))]
namespace EmirApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUserAndRoles();
        }

        public void CreateUserAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("SuperAdmin"))
            {
                //stvori super admin rolu
                var role = new IdentityRole("SuperAdmin");
                roleManager.Create(role);

                // stvaranja glavnog korisnika
                var user = new ApplicationUser();
                user.UserName = "harryBalls@gmail.com";
                user.Email = "harryBalls@gmail.com";
                string pass = "Sabanic1985";

                var newUser = userManager.Create(user, pass);
                if (newUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "SuperAdmin");
                }
            }
        }
    }
}

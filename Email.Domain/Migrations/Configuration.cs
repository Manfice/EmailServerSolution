using System.Data.Entity.Migrations;
using Email.Domain.Configurations;
using Email.Domain.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Email.Domain.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            const string role = "Administrators";
            const string userName = "Admin@admin.com";
            const string password = "mX>Jzp!FnR?5F>-4";
            const string mail = "c592@yandex.ru";

            if (!roleManager.RoleExists(role))
            {
                roleManager.Create(new ApplicationRole(role));
            }

            var user = userManager.FindByName(userName);

            if (user == null)
            {
                userManager.Create(new ApplicationUser {UserName = userName, Email = mail}, password);
                user = userManager.FindByName(userName);
            }

            if (!userManager.IsInRole(user.Id, role))
            {
                userManager.AddToRole(user.Id, role);
            }

            context.SaveChanges();
        }
    }
}

using System.Web;
using System.Web.Mvc;
using Email.Domain.Configurations;
using Microsoft.AspNet.Identity.Owin;

namespace Email.Service.Web.Controllers
{
    public class ControllerBase:Controller
    {
        protected ApplicationUserManager UserManager()
        {
            return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }
}
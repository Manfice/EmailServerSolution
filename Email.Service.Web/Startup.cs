using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Email.Service.Web.Startup))]
namespace Email.Service.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

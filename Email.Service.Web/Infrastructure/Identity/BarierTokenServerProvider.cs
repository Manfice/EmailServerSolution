using System;
using System.Threading.Tasks;
using Email.Domain.Configurations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Email.Service.Web.Infrastructure.Identity
{
    public class BarierTokenServerProvider:OAuthAuthorizationServerProvider
    {
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var uManager = context.OwinContext.Get<ApplicationUserManager>("AspNet.Identity.Owin:"+typeof(ApplicationUserManager).AssemblyQualifiedName);
            var usr = await uManager.FindAsync(context.UserName, context.Password);
            if (usr == null)
            {
                context.SetError("invalid_name_or_pass", "Неправильно указан e-mail или пароль.");
            }
            else
            {
                var ident = await uManager.CreateIdentityAsync(usr, "Custom");
                var authTicket = new AuthenticationTicket(ident, new AuthenticationProperties {ExpiresUtc =DateTimeOffset.Now.AddMinutes(300)});
                context.Validated(authTicket);
                context.Request.Context.Authentication.SignIn(ident);
            }

        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }
    }
}
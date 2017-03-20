using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Email.Agent;
using Email.DataAccess.IRepositories;

namespace Email.Service.Web.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class MailTelemetryController : ApiController
    {
        private readonly IUser _user;

        public MailTelemetryController(IUser repoUser)
        {
            _user = repoUser;
        }

        public IHttpActionResult GetResp()
        {
            var result = EmailAgent.TestEmails();
            return Ok(result);
        }

        public IHttpActionResult GetBadResp()
        {
            var result = EmailAgent.BadRequests;
            return Ok(result);
        }
    }
}

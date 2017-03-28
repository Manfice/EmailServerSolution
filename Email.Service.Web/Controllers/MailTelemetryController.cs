using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Email.Agent;
using Email.Agent.TestData;
using Email.DataAccess.IRepositories;

namespace Email.Service.Web.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class MailTelemetryController : ApiController
    {
        private readonly IMailService _mailService;

        public MailTelemetryController(IMailService repoMailService)
        {
            _mailService = repoMailService;
        }

        public IHttpActionResult GetResp()
        {
            var result = EmailAgentManager.SuccessRequests;//EmailAgent.TestEmails();
            return Ok(result);
        }

        public IHttpActionResult GetBadResp()
        {
            var result = EmailAgentManager.BadRequests;
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult AddInQueue()
        {
            new TestEmails().AddToQueue();
            return Ok(EmailAgentManager.EmailAgents);
        }
    }
}

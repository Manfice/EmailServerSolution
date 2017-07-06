using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Email.Agent.TestData;
using Email.Domain.Context;
using PriceParse;

namespace Email.Service.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx = ApplicationDbContext.Create();
        //Regex xlsRegex = new Regex("^.*\\.(xls|xlsx)$");
        public ActionResult Index()
        {
            new TestEmails().AddToQueue();
            return View(UserManager().Users.ToList());
        }

        public ActionResult About()
        {
            var r =
                PriceManager.ReadWorkbook(
                    _ctx.EmailMessages.Where(f => f.FileAttachName.EndsWith("xls"))
                        .Select(x => x.FileAttachName)
                        .ToList());
            return View(r);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
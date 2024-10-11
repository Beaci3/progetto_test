using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Useronline()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult LogoutUser(string id)
        {
            UserOnlineAttribute.LogOffUser(id);
            Thread.Sleep(1500);
            return JsonResultTrue("Utente e stato espulso");
        }
    }
}
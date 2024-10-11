using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class InstantMessageController : BaseController
    {
        // GET: Admin/InstantMessage
        public ActionResult Index()
        {
            return AjaxView();
        }

        [ValidateInput(false)]
        public ActionResult Invia(InstantMessageModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
                context.Clients.All.onSendInstantMessage("<strong>Messaggio dal Amministratore Eblig</strong><br/><br/>" + model.Messaggio);

                return JsonResultTrue("Messaggio istantaneo inviato a tutti client connessi");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
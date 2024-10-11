using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Controllers
{
    public class ErrorManagerController : BaseController
    {
        // GET: ErrorManager
        public ActionResult Index()
        {
            return AjaxView();
        }
        public ActionResult Fire404Error()
        {
            return AjaxView();
        }
    }
}
using System;
using System.Web.Mvc;

namespace EBLIG.WebUI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MaxJsonSizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            JsonResult json = filterContext.Result as JsonResult;
            if (json != null)
            {
                json.MaxJsonLength = int.MaxValue;
            }
        }
    }
}
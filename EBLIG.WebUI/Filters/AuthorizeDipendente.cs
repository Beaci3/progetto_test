using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Filters
{
    /// <summary>
    /// Autorizza ruolo Admin e Azienda
    /// </summary>
    public class AuthorizeDipendente : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (httpContext.User.IsInRole(IdentityHelper.Roles.Admin.ToString()) 
                || httpContext.User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {
                var _url = new UrlHelper(filterContext.HttpContext.Request.RequestContext);

                filterContext.RequestContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.HttpContext.Response.StatusDescription = "Accesso negato!";

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    JsonResult UnauthorizedResult = new JsonResult();
                    UnauthorizedResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                    {
                        //SingleAuthentication.RemoveUser(filterContext.HttpContext.User.Identity.Name);

                        filterContext.HttpContext.Session.Abandon();
                        filterContext.HttpContext.Response.Cookies.Clear();

                        UnauthorizedResult.Data = new { isAuthorized = false, redirectUrl = _url.Action("Login", "Account") };


                        filterContext.HttpContext.Response.StatusCode = 401;
                        filterContext.HttpContext.Response.StatusDescription = "Sessione scaduta!";
                    }

                    filterContext.Result = UnauthorizedResult;
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                    {
                        //SingleAuthentication.RemoveUser(filterContext.HttpContext.User.Identity.Name);

                        filterContext.HttpContext.Session.Abandon();
                        filterContext.HttpContext.Response.Cookies.Clear();

                        filterContext.HttpContext.Response.StatusCode = 401;
                        filterContext.HttpContext.Response.StatusDescription = "Sessione scaduta!";
                    }

                    //filterContext.HttpContext.Response.Write("Si e verificato un errore!");
                    //filterContext.HttpContext.Response.End();
                    base.HandleUnauthorizedRequest(filterContext);
                }

            }
            catch (Exception)
            {


            }
        }
    }
}
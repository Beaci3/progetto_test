using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Windows.Input;

namespace EBLIG.WebUI.Filters
{
    /// <summary>
    /// verifica che utente ha creato la sua scheda anagrafica
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CompletaRegistrazioneAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated
                    && !filterContext.HttpContext.User.IsInRole(IdentityHelper.Roles.Admin.ToString())
                    && filterContext.HttpContext.Request.Url.AbsolutePath.ToString().StartsWith("/Backend/", StringComparison.OrdinalIgnoreCase))
                {
                    var _key = "InformazioniPersonaliCompilati_" + filterContext.HttpContext.User.Identity.Name;

                    bool informazioniPersonaliCompilati()
                    {
                        try
                        {
                            if (filterContext.HttpContext.Session?[_key] == null)
                            {
                                return false;
                            }

                            return (bool)filterContext.HttpContext.Session[_key];
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    };

                    bool contains(string r, string v)
                    {
                        try
                        {
                            return filterContext.RouteData.Values[r].ToString().ToUpper() == v.ToUpper();
                        }
                        catch
                        {
                            return true;
                        }
                    };

                    if (!informazioniPersonaliCompilati())
                    {
                        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                        var _user = manager.FindByName(filterContext.HttpContext.User.Identity.Name);

                        if (filterContext.HttpContext.Session != null)
                        {
                            filterContext.HttpContext.Session[_key] = _user?.InformazioniPersonaliCompilati;
                        }

                        //action abilitati senza verifica
                        if ((contains("action", "SideMenu")
                            || contains("action", "NavMenu")
                            || contains("action", "ModificaPassword")
                            || contains("action", "GetRegioni")
                            || contains("action", "GetProvince")
                            || contains("action", "GetComuni")
                            || contains("action", "GetLocalita")
                            || contains("action", "GetProvinceByCodReg")))
                        {
                            base.OnActionExecuting(filterContext);
                            return;
                        }

                        if (!_user.InformazioniPersonaliCompilati)
                        {
                            if (!contains("action", "Anagrafica"))
                            {
                                var _role = "";
                                foreach (var item in typeof(IdentityHelper.Roles).GetEnumNames())
                                {
                                    if (filterContext.HttpContext.User.IsInRole(item))
                                    {
                                        _role = item;
                                        break;
                                    }
                                }

                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                {
                                    controller = _role,
                                    action = "Anagrafica",
                                    area = "Backend",
                                }));

                                base.OnActionExecuting(filterContext);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index",
                    area = "Backend",
                }));

                base.OnActionExecuting(filterContext);
            }
        }
    }
}



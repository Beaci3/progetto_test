using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Hubs;
using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Windows.Input;

namespace EBLIG.WebUI.Filters
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserOnlineAttribute : ActionFilterAttribute
    {
        public static List<(string, DateTime)> Useronline;

        //static object _lock = new object();

        void LogUser(ActionExecutingContext filtercontext)
        {
            Task.Run(async () =>
            {
                try
                {
                    HttpContextBase httpContext = filtercontext.HttpContext;

                    if (string.IsNullOrWhiteSpace(httpContext.User?.Identity.Name))
                    {
                        return;
                    }

                    BaseController baseController = new BaseController();

                    NavigatioHistory navigatioHistory = new NavigatioHistory();
                    navigatioHistory.Data = DateTime.Now;
                    navigatioHistory.UserHostAddress = baseController.GetIP(filtercontext.HttpContext);
                    navigatioHistory.CurrentUrl = httpContext.Request?.Url.PathAndQuery;
                    navigatioHistory.Username = httpContext.User?.Identity.Name;

                    var browser = httpContext?.Request.Browser;

                    if (browser != null)
                    {
                        navigatioHistory.BrowserIsMobileDevice = browser.IsMobileDevice;
                        navigatioHistory.BrowserJScriptVersionMajor = browser.JScriptVersion.Major;
                        navigatioHistory.BrowserJScriptVersionMinor = browser.JScriptVersion.Minor;
                        navigatioHistory.BrowserMajorVersion = browser.MajorVersion;
                        navigatioHistory.BrowserMobileDeviceModel = browser.MobileDeviceModel;
                        navigatioHistory.BrowserName = browser.Browser;
                        navigatioHistory.BrowserVersion = browser.Version;
                    }

                    UnitOfWork unitOfWork = new UnitOfWork();
                    unitOfWork.NavigatioHistoryRepository.Insert(navigatioHistory);
                    await unitOfWork.SaveAsync();
                }
                catch
                {
                }
            });
        }

        internal static void LogOffUser(string id)
        {
            RemoveUser(id);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onLogOffUtente(id, "Attenzione, sei stato espulso dal Amministratore");
        }

        internal static void RemoveUser(string id)
        {
            try
            {
                //Monitor.Enter(_lock);

                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();

                try
                {
                    Useronline.Remove(Useronline.FirstOrDefault(x => x.Item1 == id));
                }
                catch
                {

                }
                finally
                {
                    context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
                    context.Clients.All.updateUserOnline(Useronline.Select(x => x.Item1).Distinct().Count());
                }
            }
            catch
            {

            }
            finally
            {
                //Monitor.Exit(_lock);
            }
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Task.Run(() =>
            {
                try
                {
                    if (Useronline == null)
                    {
                        Useronline = new List<(string, DateTime)>();
                    }

                    if (Useronline != null)
                    {
                        foreach (var item in Useronline)
                        {
                            if (item.Item2.AddMinutes(20) < DateTime.Now)
                            {
                                Useronline.Remove(item);
                            }
                        }
                    }

                    if (filterContext.HttpContext.User != null)
                    {
                        LogUser(filterContext);

                        if (filterContext.RouteData?.Values["action"]?.ToString() != "LogOff" && filterContext.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var _user = Useronline.FirstOrDefault(x => x.Item1 == filterContext.HttpContext.User?.Identity?.Name);
                            if (_user.Item1 != null)
                            {
                                Useronline.Remove(_user);
                            }

                            Useronline.Add((filterContext.HttpContext?.User.Identity.Name, DateTime.Now));
                        }
                    }

                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
                    context.Clients.All.updateUserOnline(Useronline.Select(x => x.Item1).Distinct().Count());
                }
                catch
                {
                }
                finally
                {
                }
            });
        }
    }
}



using EBLIG.DOM.DAL;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.DataBinders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EBLIG.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            //Application.Lock();
            //Application["useronline"] = 0;
            //Application.UnLock();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception exception = Server.GetLastError();
                //string text2 =  " (0x" + exception.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
                if (exception != null)
                {
                    BaseController baseController = new BaseController();
                    var _username = User != null && User.Identity.IsAuthenticated ? User.Identity.Name : "";
                    var _ruolo = User != null && User.Identity.IsAuthenticated ? baseController.GetUserRole() : "";
                    Task.Run(() =>
                    {
                        try
                        {
                            baseController.unitOfWork.LogsRepository.Insert(new DOM.Entitys.Logs
                            {
                                Data = DateTime.Now,
                                Ruolo = _ruolo,
                                Username = _username,
                                Model = typeof(Exception).AssemblyQualifiedName,
                                ViewDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(exception),
                                Message = exception.Message,
                                Action = "Application_Error"
                            });
                            baseController.unitOfWork.Save();
                        }
                        catch
                        {
                        }
                    });


                }
            }
            catch (Exception EX)
            {

            }
        }

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404)
            {
                var exception = Server.GetLastError();
                var httpException = exception as HttpException;
                Response.Clear();
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "ErrorManager";
                routeData.Values["action"] = "Index";
                routeData.Values["exception"] = exception;
                Response.StatusCode = 500;

                if (httpException != null)
                {
                    Response.StatusCode = httpException.GetHttpCode();
                    switch (Response.StatusCode)
                    {
                        case 404:
                            routeData.Values["action"] = "Fire404Error";
                            break;
                    }
                }

                //Response.Write("aaaaaaaaaaaa");
                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;
                IController errormanagerController = new ErrorManagerController();
                HttpContextWrapper wrapper = new HttpContextWrapper(Context);
                var rc = new RequestContext(wrapper, routeData);
                errormanagerController.Execute(rc);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            try
            {
                //Application.Lock();
                //int.TryParse(Application["useronline"]?.ToString(), out int useronline);
                //Application["useronline"] = useronline + 1;
                //Application.UnLock();
            }
            catch
            {
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                //Application.Lock();
                //int.TryParse(Application["useronline"]?.ToString(), out int useronline);
                //Application["useronline"] = useronline - 1;
                //Application.UnLock();
            }
            catch
            {
            }
        }
    }
}

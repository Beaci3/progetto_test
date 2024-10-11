using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Sediin.MVC.HtmlHelpers
{
    public static class ViewExtensions
    {
        internal class FakeController : Controller
        {
        }

       public static string RenderViewToString(ControllerContext context,string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public static T CreateController<T>(RouteData routeData = null) where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = new T();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper;
            if (System.Web.HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            else
                throw new InvalidOperationException("Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        public static string RenderViewToString(this PartialViewResult partialView, object model = null, HttpContext context=null)//, string controllerName = null)
        {
            var httpContext = context ?? HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            ViewDataDictionary _ViewData = new ViewDataDictionary();
            _ViewData.Model = model != null ? model : partialView.Model;

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, _ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }

        public static string RenderViewToString(string viewName, object model, ControllerContext controllerContext)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controllerContext.RouteData.GetRequiredString("action");

            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "someValue");
                //controllerContext = new ControllerContext { RouteData = routeData };
                controllerContext.RouteData = routeData;
                //controller.ControllerContext = controllerContext;

                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        public static string RenderViewToString(object model, string filePath)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }


        //public static string RenderToString(this PartialViewResult partialView, object model = null)
        //{
        //    var httpContext = HttpContext.Current;

        //    if (httpContext == null)
        //    {
        //        throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
        //    }

        //    ViewDataDictionary _ViewData = new ViewDataDictionary();
        //    _ViewData.Model = model != null ? model : partialView.Model;

        //    var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

        //    var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

        //    var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

        //    var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

        //    var sb = new StringBuilder();

        //    using (var sw = new StringWriter(sb))
        //    {
        //        using (var tw = new HtmlTextWriter(sw))
        //        {
        //            view.Render(new ViewContext(controllerContext, view, _ViewData, partialView.TempData, tw), tw);
        //        }
        //    }

        //    return sb.ToString();
        //}
    }



}

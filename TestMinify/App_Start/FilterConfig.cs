using System.Web;
using System.Web.Mvc;
using WebMarkupMin.AspNet4.Mvc;

namespace TestMinify
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CompressContentAttribute());
            filters.Add(new MinifyHtmlAttribute());
            filters.Add(new MinifyXmlAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}

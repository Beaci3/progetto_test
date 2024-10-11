using EBLIG.WebUI.Filters;
using System.Web.Mvc;

namespace EBLIG.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
           // filters.Add(new EBLIGHandleErrorAttribute());
            filters.Add(new UserOnlineAttribute());
            filters.Add(new CompletaRegistrazioneAttribute());
            filters.Add(new MaxJsonSizeAttribute());
            filters.Add(new EncryptedActionParameterAttribute());
        }
    }
}

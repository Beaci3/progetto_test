using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class HomeController : BaseController
    {
        public List<MenuViewModel> VociMenu
        {
            get
            {
                try
                {
                    List<MenuViewModel> _vocimenu = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MenuViewModel>>(System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/Menu.json")));
                    return _vocimenu?.Where(r => r.Ruoli == null || r.Ruoli.Any(x => GetUserRoles().Contains(x)))?.OrderBy(o => o.Ordine).ToList();
                }
                catch
                {
                    return null;
                }
            }
            private set { }
        }

        // GET: Backend/Home
        public ActionResult Index()
        {
            return AjaxView();
        }

        public PartialViewResult SideMenu()
        {
            return PartialView("_PartialSideMenu", VociMenu);
        }

        public PartialViewResult NavMenu()
        {
            return PartialView("_PartialNavMenu", VociMenu);
        }

        public PartialViewResult HomeMenu()
        {
            return PartialView("_PartialHomeMenu", VociMenu);
        }

        public PartialViewResult AvvisoUtente()
        {
            try
            {
                var _role = GetUserRole();
                var _d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var _avvisi = unitOfWork.AvvisoUtenteRepository.Get(x => x.AvvisoUtenteRuoli.Count() == 0 || x.AvvisoUtenteRuoli.FirstOrDefault(a => a.Ruolo == _role) != null && (x.DataScadenza == null || (x.DataScadenza != null && x.DataScadenza >= _d)));

                return PartialView("_PartialAvvisoUtente", _avvisi);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult ApriAvviso(int id)
        {
            try
            {
                var _avvisi = unitOfWork.AvvisoUtenteRepository.Get(x => x.AvvisoUtenteId == id).FirstOrDefault();
                return PartialView("ApriAvviso", _avvisi);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
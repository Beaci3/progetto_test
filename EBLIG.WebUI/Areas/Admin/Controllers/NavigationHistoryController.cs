using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Areas.Backend.Controllers;
using EBLIG.WebUI.Controllers;
using System.Linq.Dynamic.Core;
using System.Web;
using DocumentFormat.OpenXml.EMMA;
using System.Collections.Generic;
using EBLIG.WebUI.Filters;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class NavigationHistoryController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            var _query = unitOfWork.NavigatioHistoryRepository.Get()
                .Select(x => x.BrowserName)?.Distinct();
            //.Select(d => d.BrowserName)
            //.Distinct()
            //.ToDictionary(x => x, x => x);

            NavigationHistoryRicercaModel model = new NavigationHistoryRicercaModel();
            model.Browser = _query;

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        public ActionResult Ricerca(NavigationHistoryRicercaModel model, int? page)
        {
            var _query = unitOfWork.NavigatioHistoryRepository.Get(RicercaFilter(model)).AsQueryable().OrderBy(HttpUtility.UrlDecode(model.Ordine));

            var _browser = _query.Select(x => x.BrowserName).Distinct();

            var _result = GeModelWithPaging<NavigationHistoryRicercaViewModel, NavigatioHistory>(page, _query.Distinct(), model, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(NavigationHistoryRicercaModel model)
        {
            var _query = from a in unitOfWork.NavigatioHistoryRepository.Get(RicercaFilter(model))
                         select new
                         {
                             a.Username,
                             a.Data,
                             a.CurrentUrl,
                             a.BrowserName
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "NavigationHistory");
        }

        private Expression<Func<NavigatioHistory, bool>> RicercaFilter(NavigationHistoryRicercaModel model)
        {
            return x => (model.Browsername != null ? x.BrowserName.Contains(model.Browsername) : true)
            && (model.Username != null ? x.Username.Contains(model.Username) : true);
        }

        #endregion

        public ActionResult Dettaglio(int id)
        {
            return AjaxView("Dettaglio", unitOfWork.NavigatioHistoryRepository.Get(x => x.NavigatioHistoryId == id).FirstOrDefault());
        }
    }
}
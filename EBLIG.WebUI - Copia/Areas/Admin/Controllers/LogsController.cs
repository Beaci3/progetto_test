using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Areas.Backend.Controllers;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [EBLIGAuthorize]
    [AuthorizeAdmin]
    public class LogsController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(LogsRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.LogsRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<LogsRicercaViewModel, Logs>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(LogsRicercaModel model)
        {
            var _query = from a in unitOfWork.LogsRepository.Get(RicercaFilter(model))
                         select new
                         {
                             a.Data,
                             a.Action,
                             a.Username,
                             a.Ruolo,
                             a.Message,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private SqlLam<Logs> RicercaFilter2(LogsRicercaModel model)
        {
            var f = new SqlLam<Logs>();

            //if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_RagioneSociale))
            //{
            //    f.And(x => x.RagioneSociale.Contains(model.AziendaRicercaModel_RagioneSociale));
            //}

            return f;
        }

        private Expression<Func<Logs, bool>> RicercaFilter(LogsRicercaModel model)
        {
            return null;
        }

        #endregion

        public ActionResult Log(int id)
        {
            return AjaxView(model: unitOfWork.LogsRepository.Get(x => x.LogsId == id).FirstOrDefault());
        }
    }
}
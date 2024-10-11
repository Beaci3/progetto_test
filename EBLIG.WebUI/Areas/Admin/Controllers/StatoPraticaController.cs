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
using System.Web.UI.WebControls;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class StatoPraticaController : BaseController
    {
        // GET: Backend/StatoPratica
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(StatoPraticaSearchModel model, int? page)
        {
            var _query = unitOfWork.StatoPraticaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<StatoPraticaModelRicercaViewModel, StatoPratica>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<StatoPratica, bool>> RicercaFilter(StatoPraticaSearchModel model)
        {
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(StatoPraticaRicercaModel model)
        {
            var _query = from a in unitOfWork.StatoPraticaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.Ordine,
                             a.ReadOnly,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<StatoPratica, bool>> RicercaFilter2(StatoPraticaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        public ActionResult Modifica(int id)
        {
            var _StatoPratica = unitOfWork.StatoPraticaRepository.Get(m => m.StatoPraticaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<StatoPraticaModel>(_StatoPratica);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(StatoPraticaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.StatoPraticaRepository.Get(m => m.StatoPraticaId == model.StatoPraticaId).FirstOrDefault();

                //check se StatoPratica esiste
                var _StatoPratica = unitOfWork.StatoPraticaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_StatoPratica.Count > 0 && model.Descrizione != _l.Descrizione)
                {
                    throw new Exception("Stato Pratica già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                _l.Ordine = model.Ordine;
                if (model.ReadOnly != null)
                {
                    _l.ReadOnly = model.ReadOnly;
                }
                else
                {
                    _l.ReadOnly = false;
                }
                unitOfWork.StatoPraticaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Stato Pratica aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
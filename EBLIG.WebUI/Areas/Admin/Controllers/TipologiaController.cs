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
    public class TipologiaController : BaseController
    {
        // GET: Backend/Tipologia
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TipologiaSearchModel model, int? page)
        {
            var _query = unitOfWork.TipologiaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<TipologiaModelRicercaViewModel, Tipologia>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Tipologia, bool>> RicercaFilter(TipologiaSearchModel model)
        {
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(TipologiaRicercaModel model)
        {
            var _query = from a in unitOfWork.TipologiaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.Partesociale,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Tipologia, bool>> RicercaFilter2(TipologiaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsTipologia model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _Tipologia = unitOfWork.TipologiaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_Tipologia.Count > 0)
                {
                    throw new Exception("Tipologia già presente.");
                }

                //se non esiste
                var _nuovoTipologia = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Tipologia>(model);
                _nuovoTipologia.Descrizione = model.Descrizione;
                if (model.Partesociale != null)
                {
                    _nuovoTipologia.Partesociale = model.Partesociale;
                }
                else
                {
                    _nuovoTipologia.Partesociale = false;
                }
                unitOfWork.TipologiaRepository.Insert(_nuovoTipologia);
                unitOfWork.Save();
                return JsonResultTrue("Record Tipologia inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Tipologia = unitOfWork.TipologiaRepository.Get(m => m.TipologiaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipologiaModel>(_Tipologia);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(TipologiaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.TipologiaRepository.Get(m => m.TipologiaId == model.TipologiaId).FirstOrDefault();

                //check se Tipologia esiste
                var _Tipologia = unitOfWork.TipologiaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_Tipologia.Count > 0 && model.Descrizione != _l.Descrizione)
                {
                    throw new Exception("Tipologia già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                if (model.Partesociale != null)
                {
                    _l.Partesociale = model.Partesociale;
                }
                else
                {
                    _l.Partesociale = false;
                }
                unitOfWork.TipologiaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Tipologia aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
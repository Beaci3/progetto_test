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
    public class ParentelaController : BaseController
    {
        // GET: Backend/Parentela
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(ParentelaSearchModel model, int? page)
        {
            var _query = unitOfWork.ParentelaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<ParentelaModelRicercaViewModel, Parentela>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Parentela, bool>> RicercaFilter(ParentelaSearchModel model)
        {
            ;
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(ParentelaRicercaModel model)
        {
            var _query = from a in unitOfWork.ParentelaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.Note,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Parentela, bool>> RicercaFilter2(ParentelaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsParentela model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_Parentela.Count > 0)
                {
                    throw new Exception("Parentela già presente.");
                }

                //se non esiste
                var _nuovoParentela = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Parentela>(model);
                _nuovoParentela.Descrizione = model.Descrizione;
                _nuovoParentela.Note = model.Note;
                unitOfWork.ParentelaRepository.Insert(_nuovoParentela);
                unitOfWork.Save();
                return JsonResultTrue("Parentela inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.ParentelaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<ParentelaModel>(_Parentela);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(ParentelaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.ParentelaRepository.Get(m => m.ParentelaId == model.ParentelaId).FirstOrDefault();

                //check se Motivazione esiste
                var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_Parentela.Count > 0 && model.Descrizione != _l.Descrizione)
                {
                    throw new Exception("Parentela già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                _l.Note = model.Note;
                unitOfWork.ParentelaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Parentela aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
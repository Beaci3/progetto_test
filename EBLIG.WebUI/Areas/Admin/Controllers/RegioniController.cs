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
    [EBLIGAuthorize]
    [AuthorizeAdmin]
    public class RegioniController : BaseController
    {
        // GET: Admin/Regioni
        public ActionResult Ricerca()
        {
            return AjaxView();
        }
        [HttpPost]
        public ActionResult Ricerca(RegioniModel model, int? page)
        {
            var _query = unitOfWork.RegioniRepository.Get(RicercaFilter(model)).OrderBy(m => m.DENREG);

            var _result = GeModelWithPaging<RegioniModelRicercaViewModel, Regioni>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Regioni, bool>> RicercaFilter(RegioniModel model)
        {
            ;
            return x => (model.DenReg != null ? x.DENREG.StartsWith(model.DenReg) : true);
        }

        public ActionResult RicercaExcel(RegioniRicercaModel model)
        {
            var _query = from a in unitOfWork.RegioniRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.DENREG,
                             a.CODREG,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Regioni, bool>> RicercaFilter2(RegioniRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsRegioni model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Regioni esiste
                var _Regioni = unitOfWork.RegioniRepository.Get(m => m.DENREG == model.DenReg).ToList();
                if (_Regioni.Count > 0)
                {
                    throw new Exception("Regione già presente.");
                }
                //_Regioni = unitOfWork.RegioniRepository.Get(m => m.CODREG == model.CodReg).ToList();
                //if (_Regioni.Count > 0)
                //{
                //    throw new Exception("Codice Regione già presente.");
                //}

                //se non esiste
                var _nuovoRegioni = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Regioni>(model);
                _nuovoRegioni.DENREG = model.DenReg;
                _nuovoRegioni.CODREG = unitOfWork.RegioniRepository.Get().Max(x => x.CODREG) + 1; //model.CodReg;
                _nuovoRegioni.ULTAGG = DateTime.Now;
                _nuovoRegioni.UTEAGG = "CARICAMENTO INIZIALE";
                unitOfWork.RegioniRepository.Insert(_nuovoRegioni);
                unitOfWork.Save();
                return JsonResultTrue("Regione inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Regioni = unitOfWork.RegioniRepository.Get(m => m.RegioneId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsRegioni>(_Regioni);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsRegioni model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.RegioniRepository.Get(m => m.RegioneId == model.RegioneId).FirstOrDefault();

                //check se Regione esiste
                var _Regioni = unitOfWork.RegioniRepository.Get(m => m.DENREG == model.DenReg).ToList();
                if (_Regioni.Count > 0 && model.DenReg != _l.DENREG)
                {
                    throw new Exception("Regione già presente.");
                }
                //_Regioni = unitOfWork.RegioniRepository.Get(m => m.CODREG == model.CodReg && m.RegioneId != model.RegioneId).ToList();
                //if (_Regioni.Count > 0)
                //{
                //    throw new Exception("Codice Regione già presente.");
                //}

                //se non esiste allora modifico
                _l.DENREG = model.DenReg;
                //_l.CODREG = model.CodReg;
                _l.ULTAGG = DateTime.Now;
                unitOfWork.RegioniRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Regione aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
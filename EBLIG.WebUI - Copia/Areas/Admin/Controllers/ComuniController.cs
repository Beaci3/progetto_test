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
    public class ComuniController : BaseController
    {
        // GET: Backend/Comuni
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(ComuniModel model, int? page)
        {
            var _query = unitOfWork.ComuniRepository.Get(RicercaFilter(model)).OrderBy(m => m.DENCOM);

            var _result = GeModelWithPaging<ComuniModelRicercaViewModel, Comuni>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Comuni, bool>> RicercaFilter(ComuniModel model)
        {;
            return x => (model.DenCom != null ? x.DENCOM.StartsWith(model.DenCom) : true) &&
            (model.SigPro != null ? x.SIGPRO == model.SigPro : true) &&
            (model.CodCom != null ? x.CODCOM == model.CodCom : true);
        }

        public ActionResult RicercaExcel(ComuniRicercaModel model)
        {
            var _query = from a in unitOfWork.ComuniRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.CODCOM,
                             a.DENCOM,
                             a.SIGPRO,
                             a.CODSTA,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Comuni, bool>> RicercaFilter2(ComuniRicercaModel model)
        {
            return null;
        }

        public ActionResult GetProvince(string phrase)
        {
            Expression<Func<Province, bool>> _filter = x =>
                        (phrase != null ? (x.DENPRO).StartsWith(phrase) : true);

            var _result = unitOfWork.ProvinceRepository.Get(_filter);

            if (_result.Count() > 0)
            {
                return Json(_result.Select(x => new { x.SIGPRO, x.DENPRO }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsComuni model)
        {
            try
            {
                //check se Comune esiste
                var _Comuni = unitOfWork.ComuniRepository.Get(m => m.DENCOM == model.DenCom && m.SIGPRO == model.SigPro).ToList();
                if (_Comuni.Count > 0)
                {
                    throw new Exception("Comune già presente.");
                }

                //se non esiste
                var _nuovoComune = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Comuni>(model);
                _nuovoComune.CODCOM = model.CodCom;
                _nuovoComune.DENCOM = model.DenCom;
                _nuovoComune.SIGPRO = model.SigPro;
                _nuovoComune.CODSTA = model.CodSta;
                _nuovoComune.ULTAGG = DateTime.Now;
                _nuovoComune.UTEAGG = "CARICAMENTO INIZIALE";
                unitOfWork.ComuniRepository.Insert(_nuovoComune);
                unitOfWork.Save();
                return JsonResultTrue("Comune inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Comune = unitOfWork.ComuniRepository.Get(m => m.ComuneId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsComuni>(_Comune);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsComuni model)
        {
            try
            {
                var _l = unitOfWork.ComuniRepository.Get(m => m.ComuneId == model.ComuneId).FirstOrDefault();

                //check se Comune esiste
                var _Comune = unitOfWork.ComuniRepository.Get(m => m.DENCOM == model.DenCom && m.SIGPRO == model.SigPro).ToList();
                if (_Comune.Count > 0 && model.DenCom != _l.DENCOM)
                {
                    throw new Exception("Comune già presente.");
                }

                //se non esiste allora modifico
                _l.CODCOM = model.CodCom;
                _l.DENCOM = model.DenCom;
                _l.SIGPRO = model.SigPro;
                _l.CODSTA = model.CodSta;
                _l.ULTAGG = DateTime.Now;
                unitOfWork.ComuniRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Comune aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
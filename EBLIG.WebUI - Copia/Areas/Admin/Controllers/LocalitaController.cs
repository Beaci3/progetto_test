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
    public class LocalitaController : BaseController
    {
        // GET: Backend/Localita
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(LocalitaModel model, int? page)
        {
            var _query = unitOfWork.LocalitaRepository.Get(RicercaFilter(model)).OrderBy(m => m.DENLOC);

            var _result = GeModelWithPaging<LocalitaModelRicercaViewModel, Localita>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Localita, bool>> RicercaFilter(LocalitaModel model)
        {;
            return x => (model.DenLoc != null ? x.DENLOC.StartsWith(model.DenLoc) : true) &&
            (model.SigPro != null ? x.SIGPRO == model.SigPro : true) &&
            (model.Cap != null ? x.CAP == model.Cap : true);
        }


        public ActionResult RicercaExcel(LocalitaRicercaModel model)
        {
            var _query = from a in unitOfWork.LocalitaRepository.Get(RicercaFilter(model))
                         select new
                         {
                             a.CODLOC,
                             a.CAP,
                             a.DENLOC,
                             a.SIGPRO,
                             a.CODCOM,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Localita, bool>> RicercaFilter(LocalitaRicercaModel model)
        {
            return null;
        }

        public ActionResult GetComuni(string phrase)
        {
            Expression<Func<Comuni, bool>> _filter = x =>
                        (phrase != null ? (x.DENCOM).StartsWith(phrase) : true);

            var _result = unitOfWork.ComuniRepository.Get(_filter);

            if (_result.Count() > 0)
            {
                return Json(_result.Select(x => new { x.CODCOM, x.DENCOM }), JsonRequestBehavior.AllowGet);
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
        public ActionResult Nuovo(InsLocalita model)
        {
            try
            {
                //check se località esiste
                var _localita = unitOfWork.LocalitaRepository.Get(m => m.DENLOC == model.DenLoc && m.CAP == model.Cap).ToList();
                if (_localita.Count > 0)
                {
                    throw new Exception("Località già presente.");
                }

                //se non esiste
                var _nuovaLocalita = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Localita>(model);
                _nuovaLocalita.CAP = model.Cap;
                _nuovaLocalita.DENLOC = model.DenLoc.ToUpper();
                _nuovaLocalita.SIGPRO = unitOfWork.ProvinceRepository.Get(m => m.ProvinciaId == model.ProvinciaId).FirstOrDefault().SIGPRO;
                _nuovaLocalita.CODCOM = unitOfWork.ComuniRepository.Get(m => m.ComuneId == model.ComuneId).FirstOrDefault().CODCOM;
                _nuovaLocalita.CODLOC = unitOfWork.LocalitaRepository.Get().LastOrDefault().CODLOC + 1;
                _nuovaLocalita.ULTAGG = DateTime.Now;
                _nuovaLocalita.UTEAGG = User.Identity.Name.ToUpper();
                unitOfWork.LocalitaRepository.Insert(_nuovaLocalita);
                unitOfWork.Save();
                return JsonResultTrue("Località inserita");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _localita = unitOfWork.LocalitaRepository.Get(m => m.LocalitaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsLocalita>(_localita);
            _l.ComuneId = unitOfWork.ComuniRepository.Get(m => m.CODCOM == _localita.CODCOM).FirstOrDefault().ComuneId;
            _l.ProvinciaId = unitOfWork.ProvinceRepository.Get(m => m.SIGPRO == _localita.SIGPRO).FirstOrDefault().ProvinciaId;
            _l.CodReg = unitOfWork.ProvinceRepository.Get(m => m.SIGPRO == _localita.SIGPRO).FirstOrDefault().CODREG;
            _l.RegioneId = unitOfWork.RegioniRepository.Get(m => m.CODREG == _l.CodReg).FirstOrDefault().RegioneId;
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsLocalita model)
        {
            try
            {
                var _l = unitOfWork.LocalitaRepository.Get(m => m.LocalitaId == model.LocalitaId).FirstOrDefault();
               
                //check se località esiste
                var _localita = unitOfWork.LocalitaRepository.Get(m => m.DENLOC == model.DenLoc && m.CAP == model.Cap).ToList();
                if (_localita.Count > 0 && model.DenLoc != _l.DENLOC)
                {
                    throw new Exception("Località già presente.");
                }

                //se non esiste allora modifico
                _l.DENLOC = model.DenLoc.ToUpper();
                _l.CAP = model.Cap;
                _l.SIGPRO = unitOfWork.ProvinceRepository.Get(m => m.ProvinciaId == model.ProvinciaId).FirstOrDefault().SIGPRO;
                _l.CODCOM = unitOfWork.ComuniRepository.Get(m => m.ComuneId == model.ComuneId).FirstOrDefault().CODCOM;
                _l.ULTAGG = DateTime.Now;
                _l.UTEAGG = User.Identity.Name.ToUpper();
                unitOfWork.LocalitaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Località aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
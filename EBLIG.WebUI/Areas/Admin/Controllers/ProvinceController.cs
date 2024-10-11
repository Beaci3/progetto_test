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
    public class ProvinceController : BaseController
    {
        // GET: Admin/Province
        public ActionResult Ricerca()
        {
            return AjaxView();
        }
        [HttpPost]
        public ActionResult Ricerca(ProvinceModel model, int? page)
        {
            var _query = unitOfWork.ProvinceRepository.Get(RicercaFilter(model)).OrderBy(m => m.DENPRO);

            var _result = GeModelWithPaging<ProvinceModelRicercaViewModel, Province>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Province, bool>> RicercaFilter(ProvinceModel model)
        {
            ;
            return x => (model.DenPro != null ? x.DENPRO.StartsWith(model.DenPro) : true)
                        && (model.SigPro != null ? x.SIGPRO == model.SigPro : true);

        }

        public ActionResult RicercaExcel(ProvinceRicercaModel model)
        {
            var _query = from a in unitOfWork.ProvinceRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.SIGPRO,
                             a.DENPRO,
                             a.CODREG,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Province, bool>> RicercaFilter2(ProvinceRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            InsProvince _Regioni = new InsProvince();
            _Regioni.Regioni = unitOfWork.RegioniRepository.Get();

            return AjaxView("Nuovo", _Regioni);
        }

        [HttpPost]
        public ActionResult Nuovo(InsProvince model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Province esiste
                var _Province = unitOfWork.ProvinceRepository.Get(m => m.DENPRO == model.DenPro && m.SIGPRO == model.SigPro).ToList();
                if (_Province.Count > 0)
                {
                    throw new Exception("Provincia già presente.");
                }
                _Province = unitOfWork.ProvinceRepository.Get(m => m.SIGPRO == model.SigPro).ToList();
                if (_Province.Count > 0)
                {
                    throw new Exception("Sigla Provincia già presente.");
                }

                //se non esiste
                var _nuovoProvince = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Province>(model);
                _nuovoProvince.SIGPRO = model.SigPro;
                _nuovoProvince.DENPRO = model.DenPro;
                _nuovoProvince.CODREG = model.CodReg;
                _nuovoProvince.ULTAGG = DateTime.Now;
                _nuovoProvince.UTEAGG = "CARICAMENTO INIZIALE";
                unitOfWork.ProvinceRepository.Insert(_nuovoProvince);
                unitOfWork.Save();
                return JsonResultTrue("Provincia inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Province = unitOfWork.ProvinceRepository.Get(m => m.ProvinciaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsProvince>(_Province);
            _l.Regioni = unitOfWork.RegioniRepository.Get();
            _l.RegioneId = unitOfWork.RegioniRepository.Get(m => m.CODREG == _l.CodReg).FirstOrDefault().RegioneId;
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsProvince model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.ProvinceRepository.Get(m => m.ProvinciaId == model.ProvinciaId).FirstOrDefault();

                //check se Comune esiste
                var _Province = unitOfWork.ProvinceRepository.Get(m => m.DENPRO == model.DenPro && m.SIGPRO == model.SigPro).ToList();
                if (_Province.Count > 0 && model.DenPro != _l.DENPRO)
                {
                    throw new Exception("Provincia già presente.");
                }
                _Province = unitOfWork.ProvinceRepository.Get(m => m.SIGPRO == model.SigPro && m.ProvinciaId != model.ProvinciaId)?.ToList();
                if (_Province?.Count > 0)
                {
                    throw new Exception("Sigla Provincia già Utilizzata.");
                }

                //se non esiste allora modifico
                _l.SIGPRO = model.SigPro;
                _l.DENPRO = model.DenPro;
                _l.CODREG = model.CodReg;
                _l.ULTAGG = DateTime.Now;
                unitOfWork.ProvinceRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Provincia aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
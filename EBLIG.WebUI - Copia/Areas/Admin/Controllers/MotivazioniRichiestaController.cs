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
    public class MotivazioniRichiestaController : BaseController
    {
        // GET: Backend/Motivazioni
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(MotivazioniRichiestaModel model, int? page)
        {
            var _query = unitOfWork.MotivazioniRichiestaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Motivazione);

            var _result = GeModelWithPaging<MotivazioniRichiestaModelRicercaViewModel, MotivazioniRichiesta>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<MotivazioniRichiesta, bool>> RicercaFilter(MotivazioniRichiestaModel model)
        {
            ;
            return x => model.Motivazione != null ? x.Motivazione.StartsWith(model.Motivazione) : true
                        && (model.MotivazioniRichiestaRicercaModel_TipoRichiestaId != null ? x.TipoRichiestaId == model.MotivazioniRichiestaRicercaModel_TipoRichiestaId : true);

        }

        public ActionResult RicercaExcel(MotivazioniRichiestaRicercaModel model)
        {
            var _query = from a in unitOfWork.MotivazioniRichiestaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Motivazione,
                             a.TipoRichiestaId,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<MotivazioniRichiesta, bool>> RicercaFilter2(MotivazioniRichiestaRicercaModel model)
        {
            return null;
        }

        //public ActionResult GetTipoRichiestaId(string phrase)
        //{
        //    Expression<Func<TipoRichiesta, bool>> _filter = x =>
        //                (phrase != null ? (x.Descrizione).StartsWith(phrase) : true);

        //    var _result = unitOfWork.TipoRichiestaRepository.Get(_filter);

        //    if (_result.Count() > 0)
        //    {
        //        return Json(_result.Select(x => new { x.TipoRichiestaId, x.Descrizione }), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult Nuovo()
        {
            var model = new InsMotivazioniRichiesta();
            model.TipoRichiesta = unitOfWork.TipoRichiestaRepository.Get().ToList();

            return AjaxView("Nuovo", model);
        }

        [HttpPost]
        public ActionResult Nuovo(InsMotivazioniRichiesta model)
        {
            try
            {
                //check se Motivazione esiste
                var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                if (_Motivazioni.Count > 0)
                {
                    throw new Exception("Motivazione Richiesta già presente.");
                }

                //se non esiste
                var _nuovoMotivazioni = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<MotivazioniRichiesta>(model);
                _nuovoMotivazioni.TipoRichiestaId = model.TipoRichiestaId;
                _nuovoMotivazioni.Motivazione = model.Motivazione;
                unitOfWork.MotivazioniRichiestaRepository.Insert(_nuovoMotivazioni);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione Richiesta inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.MotivazioniRichiestaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsMotivazioniRichiesta>(_Motivazioni);
            _l.TipoRichiesta = unitOfWork.TipoRichiestaRepository.Get();

            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsMotivazioniRichiesta model)
        {
            try
            {
                var _l = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.MotivazioniRichiestaId == model.MotivazioniRichiestaId).FirstOrDefault();

                //check se Motivazione esiste
                var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                if (_Motivazioni.Count > 0 && model.Motivazione != _l.Motivazione)
                {
                    throw new Exception("Motivazione Richiesta già presente.");
                }

                //se non esiste allora modifico
                _l.TipoRichiestaId = model.TipoRichiestaId;
                _l.Motivazione = model.Motivazione;
                unitOfWork.MotivazioniRichiestaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione Richiesta aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
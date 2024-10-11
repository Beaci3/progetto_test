using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class AllegatiController : BaseController
    {
        // GET: Backend/TipoRichieste
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(AllegatiClass model)
        {
            var _allegati = unitOfWork.AllegatiRepository.Get(RicercaFilter(model)).OrderBy(m => m.Nome);
            return AjaxView("RicercaList", _allegati);
        }

        private Expression<Func<EBLIG.DOM.Entitys.Allegati, bool>> RicercaFilter(AllegatiClass model)
        {
            return x => (model.Nome != null ? x.Nome.Contains(model.Nome) : true);
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(AllegatiClass model)
        {
            try
            {
                //check se allegato esiste
                var _allegati = unitOfWork.AllegatiRepository.Get(m => m.Nome == model.Nome).ToList();
                if (_allegati.Count > 0)
                {
                    throw new Exception("Allegato già presente.");
                }

                //se non esiste
                var _nuovoAllegato = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<EBLIG.DOM.Entitys.Allegati>(model);
                _nuovoAllegato.Nome = model.Nome;
                unitOfWork.AllegatiRepository.Insert(_nuovoAllegato);
                unitOfWork.Save();
                return JsonResultTrue("Allegato inserito");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _allegato = unitOfWork.AllegatiRepository.Get(m => m.AllegatoId == id).FirstOrDefault();
            return AjaxView("Modifica", _allegato);
        }

        [HttpPost]
        public ActionResult Modifica(DOM.Entitys.Allegati model)
        {
            try
            {
                var _a = unitOfWork.AllegatiRepository.Get(m => m.AllegatoId == model.AllegatoId).FirstOrDefault();

                //check se allegato esiste
                var _allegati = unitOfWork.AllegatiRepository.Get(m => m.Nome == model.Nome).ToList();
                if (_allegati.Count > 0 && model.Nome != _a.Nome)
                {
                    throw new Exception("Allegato già presente.");
                }

                //se non esiste allora modifico
                _a.Nome = model.Nome;
                unitOfWork.AllegatiRepository.Update(_a);
                unitOfWork.Save();
                return JsonResultTrue("Allegato aggiornato");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int allegatoId)
        {
            var model = new EliminaAllegato();
            model.allegatoId = allegatoId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.Allegati model)
        {
            try
            {
                var tipoRichieste = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.AllegatoId == model.AllegatoId);
                //check se allegato è associato a tipo richiesta
                if (tipoRichieste.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: allegato già associato a tipo richiesta");
                }

                //se non usata
                unitOfWork.AllegatiRepository.Delete(model.AllegatoId);
                unitOfWork.Save();
                return JsonResultTrue("Eliminazione effettuata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
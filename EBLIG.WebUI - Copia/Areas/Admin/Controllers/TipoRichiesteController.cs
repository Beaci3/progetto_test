using EBLIG.DOM.Entitys;
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
    public class TipoRichiesteController : BaseController
    {
        // GET: Backend/TipoRichieste
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TipoRichiesteRicercaModel model)
        {
            var _tiporich = unitOfWork.TipoRichiestaRepository.Get(RicercaFilter(model)).OrderByDescending(m => m.Anno).ThenBy(m => m.Descrizione);


            return AjaxView("RicercaList", _tiporich);
        }


        public ActionResult Modifica(int id)
        {
            var _tipo = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == id).FirstOrDefault();

            return AjaxView("Modifica", _tipo);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Modifica(TipoRichiesta model)
        {
            try
            {
                var _t = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();
                _t.Descrizione = model.Descrizione;
                _t.Note = model.Note;
                _t.Contributo = model.Contributo;
                _t.Anno = model.Anno;
                _t.MaxRichiesteAnno = model.MaxRichiesteAnno;
                if (model.CoperturaMatricolaInps != null)
                {
                    _t.CoperturaMatricolaInps = model.CoperturaMatricolaInps;
                }
                else
                {
                    _t.CoperturaMatricolaInps = false;
                }
                if (model.AbilitatoNuovaRichiesta != null)
                {
                    _t.AbilitatoNuovaRichiesta = model.AbilitatoNuovaRichiesta;
                }
                else
                {
                    _t.AbilitatoNuovaRichiesta = false;
                }                
                if (model.IsTipoRichiestaDipendente != null)
                {
                    _t.IsTipoRichiestaDipendente = model.IsTipoRichiestaDipendente;
                }
                else
                {
                    _t.IsTipoRichiestaDipendente = false;
                }
                _t.AliquoteIRPEF = model.AliquoteIRPEF;
                unitOfWork.TipoRichiestaRepository.Update(_t);
                unitOfWork.Save();
                return JsonResultTrue("Tipo richiesta aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        private Expression<Func<TipoRichiesta, bool>> RicercaFilter(TipoRichiesteRicercaModel model)
        {
            return x => (model.Anno != null ? x.Anno == model.Anno : true)
            && (model.AbilitatoNuovaRichiesta != null ? (model.AbilitatoNuovaRichiesta == "0" ? x.AbilitatoNuovaRichiesta == false : x.AbilitatoNuovaRichiesta == true) : true)
            && (model.IsTipoRichiestaDipendente != null ? (model.IsTipoRichiestaDipendente == "0" ? x.IsTipoRichiestaDipendente == false : x.IsTipoRichiestaDipendente == true) : true);
        }

        [HttpPost]
        public ActionResult Allegati(AllegatiModel model)
        {
            try
            {
                var _t = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).ToList();

                if (_t.Count() > 0)
                {
                    foreach (var item in _t.ToList())
                    {
                        unitOfWork.TipoRichiestaAllegatiRepository.Delete(item);
                    }
                }

                if (model.Allegati.Count() > 0)
                {
                    foreach (var item in model.Allegati)
                    {
                        if (item.Selezionato == false)
                        {
                            continue;
                        }
                        var _a = new TipoRichiestaAllegati();
                        _a.TipoRichiestaId = model.TipoRichiestaId;
                        _a.AllegatoId = item.AllegatoId;
                        _a.Obblicatorio = item.Obbligatorio;
                        unitOfWork.TipoRichiestaAllegatiRepository.Insert(_a);
                    }
                }

                unitOfWork.Save();

                return JsonResultTrue("Allegati per tipo richiesta aggiornati");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Allegati(int tipoRichiestaId)
        {

            var _allegatiTipoRich = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId);

            var _allegati = from x in unitOfWork.AllegatiRepository.Get()
                            select new Models.Allegati
                            {
                                AllegatoId = x.AllegatoId,
                                Nome = x.Nome,
                                Obbligatorio = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null ? _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId).Obblicatorio.GetValueOrDefault() : false,
                                Selezionato = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null,
                                TipoRichiestaId = tipoRichiestaId
                            };

            var model = new AllegatiModel();
            model.Allegati = _allegati;
            model.TipoRichiestaId = tipoRichiestaId;

            return AjaxView("Allegati", model);
        }

        public ActionResult Duplica(int tipoRichiestaId, int anno)
        {
            var model = new Duplica();
            model.TipoRichiestaId = tipoRichiestaId;
            model.Anno = anno;
            return AjaxView("Duplica", model);
        }

        [HttpPost]
        public ActionResult Duplica(TipoRichiesta model)
        {
            try
            {
                var tipoRichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();

                //check se anno esiste
                var t = unitOfWork.TipoRichiestaRepository.Get(x => x.Descrizione == tipoRichiesta.Descrizione && x.Anno == model.Anno);
                if (t.Count() > 0)
                {
                    throw new Exception("Tipo richiesta già presente per l'anno " + model.Anno);
                }

                //se non esiste per quell'anno
                var _nuovaRichiesta = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoRichiesta>(tipoRichiesta);
                _nuovaRichiesta.Anno = model.Anno;
                unitOfWork.TipoRichiestaRepository.Insert(_nuovaRichiesta);
                unitOfWork.Save();
                var identity = _nuovaRichiesta.TipoRichiestaId;
                var _azioni = unitOfWork.AzioniPraticaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_azioni.Count() > 0)
                {
                    foreach (var a in _azioni)
                    {
                        a.TipoRichiestaId = identity;
                        unitOfWork.AzioniPraticaRepository.Insert(a);
                    }
                }
                var _motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_motivazioni.Count() > 0)
                {
                    foreach (var m in _motivazioni)
                    {
                        m.TipoRichiestaId = identity;
                        unitOfWork.MotivazioniRichiestaRepository.Insert(m);
                    }
                }
                var _allegati = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_allegati.Count() > 0)
                {
                    foreach (var all in _allegati)
                    {
                        all.TipoRichiestaId = identity;
                        unitOfWork.TipoRichiestaAllegatiRepository.Insert(all);
                    }
                }
                unitOfWork.Save();
                return JsonResultTrue("Duplicazione per l'anno " + model.Anno + " effettuata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int tipoRichiestaId)
        {
            var model = new Elimina();
            model.TipoRichiestaId = tipoRichiestaId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(TipoRichiesta model)
        {
            try
            {
                var pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                //check se tipologia è usata da pratica
                if (pratiche.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: tipo richiesta già utilizzata per una pratica");
                }

                //se non usata
                unitOfWork.TipoRichiestaRepository.Delete(model.TipoRichiestaId);
                var _azioni = unitOfWork.AzioniPraticaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_azioni.Count() > 0)
                {
                    foreach (var a in _azioni)
                    {
                        unitOfWork.AzioniPraticaRepository.Delete(a);
                    }
                }
                var _motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_motivazioni.Count() > 0)
                {
                    foreach (var m in _motivazioni)
                    {
                        unitOfWork.MotivazioniRichiestaRepository.Delete(m);
                    }
                }
                var _allegati = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_allegati.Count() > 0)
                {
                    foreach (var all in _allegati)
                    {
                        unitOfWork.TipoRichiestaAllegatiRepository.Delete(all);
                    }
                }
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
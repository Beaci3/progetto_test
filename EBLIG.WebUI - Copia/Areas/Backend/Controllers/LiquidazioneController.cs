using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM;
using EBLIG.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using System.IO;
using DocumentFormat.OpenXml.EMMA;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;
using EBLIG.DOM.Providers;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [AuthorizeAdmin]
    public class LiquidazioneController : BaseController
    {
        public string PathLiquidazione { get => "PraticheAzienda\\Liquidazione\\{0}"; private set { } }

        public List<int> ListaPraticheLiquidazione
        {

            get
            {
                Session.Timeout = 20;

                List<int> lista = new List<int>();

                if (Session["ListaPraticheLiquidazione"] != null)
                {
                    lista = (List<int>)Session["ListaPraticheLiquidazione"];
                }

                Session["ListaPraticheLiquidazione"] = lista;

                return lista;
            }

            set
            {
                Session["ListaPraticheLiquidazione"] = value;

            }
        }

        #region ricerca

        public ActionResult Ricerca()
        {
            LiquidazioneRicercaModel model = new LiquidazioneRicercaModel
            {
                StatoLiquidazione = unitOfWork.StatoLiquidazioneRepository.Get().ToList()
            };

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        public ActionResult Ricerca(LiquidazioneRicercaModel model, int? page)
        {
            var _query = unitOfWork.LiquidazionePraticheRegionaliRepository.Get(RicercaFilter(model))
            .OrderBy(x => x.PraticheRegionaliImprese.DataInvio == null).ThenBy(x => x.PraticheRegionaliImprese.DataInvio).ThenBy(x => x.PraticheRegionaliImprese.DataInserimento);

            var _netto = _query?.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto);
            var _liquidati = _query?.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.Liquidata)?.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto);
            var _inliquidazione = _query?.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.InLiquidazione)?.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto);
            var _annullato = _query?.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.Annullata)?.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto);

            var _queryLiq = unitOfWork.LiquidazioneRepository.Get(RicercaFilterLiq(model)).OrderByDescending(d => d.DataCreazione);

            var _result = GeModelWithPaging<LiquidazioneRicercaViewModel, Liquidazione>(page, _queryLiq, model, 10);
            _result.ImportoDaLiquidare = _netto;
            _result.ImportoAnnullato = _annullato;
            _result.ImportoInLiquidazione = _inliquidazione;
            _result.ImportoLiquidato = _liquidati;
            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(LiquidazioneRicercaModel model)
        {
            var _query = from a in unitOfWork.LiquidazionePraticheRegionaliRepository.Get(RicercaFilter(model))
                         .Select(x => x.Liquidazione)
                         select new
                         {
                             Stato = a.StatoLiquidazione.Descrizione,
                             NumeroLiquidazione = a.LiquidazioneId.ToString().PadLeft(7, '0'),
                             Importo = a.LiquidazionePraticheRegionali?.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto).GetValueOrDefault().ToString("n"),
                             DataCreazione = a.DataCreazione.ToShortDateString(),
                             DataLavorazione = a.DataLavorazione.HasValue ? a.DataLavorazione.GetValueOrDefault().ToShortDateString() : ""
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Liquidazioni");
        }

        private Expression<Func<Liquidazione, bool>> RicercaFilterLiq(LiquidazioneRicercaModel model)
        {

            if (model.LiquidazioneRicercaModel_StatoLiquidazioneId != null)
            {
                return x => (model.LiquidazioneRicercaModel_StatoLiquidazioneId != null ? x.StatoLiquidazioneId == (int)model.LiquidazioneRicercaModel_StatoLiquidazioneId : true);
            }

            return null; ;

        }

        private Expression<Func<LiquidazionePraticheRegionali, bool>> RicercaFilter(LiquidazioneRicercaModel model)
        {
            if (model.LiquidazioneRicercaModel_StatoLiquidazioneId != null)
            {
                return x => (model.LiquidazioneRicercaModel_StatoLiquidazioneId != null ? x.Liquidazione.StatoLiquidazioneId == (int)model.LiquidazioneRicercaModel_StatoLiquidazioneId : true);
            }

            return null; ;
        }

        #endregion

        #region crea liquidazione

        public ActionResult RicercaDaLiquidare()
        {
            var result = unitOfWork.TipoRichiestaRepository.Get().ToList();

            if (result != null)
            {
                foreach (var item in result)
                {
                    item.Descrizione = item.Descrizione + " (" + item.Anno + ")";
                }
            }

            LiquidazioneDaLiquidareRicercaModel model = new LiquidazioneDaLiquidareRicercaModel
            {
                ListaPraticheLiquidazione = ListaPraticheLiquidazione?.Count(),
                TipoRichiesta = result?.OrderBy(x => x.Descrizione)?.ToList()
            };

            return AjaxView("RicercaDaLiquidare", model);
        }

        [HttpPost]
        public ActionResult RicercaDaLiquidare(LiquidazioneDaLiquidareRicercaModel model, int? page)
        {
            var _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaDaLiquidareFilter(model))
                .OrderBy(x => x.DataInvio == null).ThenBy(x => x.DataInvio).ThenBy(x => x.DataInserimento);
            
            var _result = GeModelWithPaging<LiquidazioneDaLiquidareRicercaViewModel, PraticheRegionaliImprese>(page, _query, model, 10);
            _result.ImportoDaLiquidare = _query.Sum(x => x.ImportoContributoNetto);
            _result.ListaPraticheLiquidazione = ListaPraticheLiquidazione;
            _result.ImportoListaPraticheLiquidazione = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => ListaPraticheLiquidazione.Contains(xx.PraticheRegionaliImpreseId))?.Sum(x => x.ImportoContributoNetto);

            return AjaxView("RicercaDaLiquidareList", _result);
        }

        private Expression<Func<PraticheRegionaliImprese, bool>> RicercaDaLiquidareFilter(LiquidazioneDaLiquidareRicercaModel model)
        {
            var _liquidatiOinliquidazione = unitOfWork.LiquidazionePraticheRegionaliRepository.Get(xx =>
            xx.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.InLiquidazione
            || xx.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.Liquidata).Select(x => x.PraticheRegionaliImpreseId);

            return x =>
            (x.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata) && (!_liquidatiOinliquidazione.Contains(x.PraticheRegionaliImpreseId))
            && ((model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId != 0 && model.PraticheAziendaRicercaModel_TipoRichiestaId != -1) ? x.TipoRichiestaId == model.PraticheAziendaRicercaModel_TipoRichiestaId : true)
            && (model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 || model.PraticheAziendaRicercaModel_TipoRichiestaId == -1) ? (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 ? x.TipoRichiesta.IsTipoRichiestaDipendente != true : x.TipoRichiesta.IsTipoRichiestaDipendente == true) : true)
            && (model.PraticheAziendaRicercaModel_AziendaId != null ? x.AziendaId == model.PraticheAziendaRicercaModel_AziendaId : true)
            && (model.PraticheAziendaRicercaModel_DipendenteId != null ? x.DipendenteId == model.PraticheAziendaRicercaModel_DipendenteId : true));
        }

        [HttpPost]
        public ActionResult CreaListaLiquidazione()
        {
            unitOfWork.LiquidazioneRepository.Insert(new Liquidazione
            {
                DataCreazione = DateTime.Now,
                StatoLiquidazioneId = (int)EbligEnums.StatoLiqidazione.InLiquidazione,
                LiquidazionePraticheRegionali = ListaPraticheLiquidazione.Select(x => new LiquidazionePraticheRegionali
                {
                    PraticheRegionaliImpreseId = x
                }).ToList()
            });

            unitOfWork.Save();

            ListaPraticheLiquidazione = null;

            return JsonResultTrue("Lista creata");
        }

        [HttpPost]
        public ActionResult RimuoviRichiesta()
        {
            ListaPraticheLiquidazione = null;

            return JsonResultTrue("Lista cancellata");
        }

        [HttpPost]
        public ActionResult AggiungiRimuoviRichiesta(int id)
        {
            if (ListaPraticheLiquidazione.Where(x => x == id).Count() == 0)
            {
                ListaPraticheLiquidazione.Add(id);
            }
            else
            {
                ListaPraticheLiquidazione.Remove(id);
            }

            var _importo = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => ListaPraticheLiquidazione.Contains(xx.PraticheRegionaliImpreseId))?.Sum(x => x.ImportoContributoNetto);

            return Json(new { isValid = true, importo = _importo?.ToString("n"), totali = ListaPraticheLiquidazione.Count() });
        }

        #endregion

        public ActionResult ApriLiquidazione(int id)
        {
            LiquidazioneViewModel model = new LiquidazioneViewModel
            {
                Liquidazione = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == id).FirstOrDefault(),
                StatoLiquidazione = unitOfWork.StatoLiquidazioneRepository.Get().ToList()
            };

            return AjaxView("ApriLiquidazione", model);
        }

        [HttpPost]
        public ActionResult RimuoviRigaLiquidazione(int liquidazioneId, int praticheRegionaliImpreseId)
        {
            try
            {
                var _liquidazioneriga = unitOfWork.LiquidazionePraticheRegionaliRepository.Get(x => x.LiquidazioneId == liquidazioneId && x.PraticheRegionaliImpreseId == praticheRegionaliImpreseId).FirstOrDefault();
                unitOfWork.LiquidazionePraticheRegionaliRepository.Delete(_liquidazioneriga.LiquidazionePraticheRegionaliId);
                unitOfWork.Save();

                return JsonResultTrue("Prestazione Regionale cancellata");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LavoraLiquidazione(int liquidazioneId)
        {
            LiquidazioneLavoraViewModel model = new LiquidazioneLavoraViewModel();
            model.LiquidazioneId = liquidazioneId;
            return AjaxView("LavoraLiquidazione", model);
        }

        [HttpGet]
        public ActionResult AnnullaLiquidazione(int liquidazioneId)
        {
            LiquidazioneAnnullaViewModel model = new LiquidazioneAnnullaViewModel();
            model.LiquidazioneId = liquidazioneId;
            return AjaxView("AnnullaLiquidazione", model);
        }

        [HttpPost]
        public ActionResult LavoraLiquidazione(LiquidazioneLavoraViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelStateErrorToString(ModelState));
            }

            ElaboraLiquidazione(model.LiquidazioneId, model.Note, model.Allegato, (int)EbligEnums.StatoLiqidazione.Liquidata);

            return JsonResultTrue("Stato Liquidazione aggiornata");
        }

        [HttpPost]
        public ActionResult AnnullaLiquidazione(LiquidazioneAnnullaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelStateErrorToString(ModelState));
            }

            ElaboraLiquidazione(model.LiquidazioneId, model.Note, model.Allegato, (int)EbligEnums.StatoLiqidazione.Annullata);

            return JsonResultTrue("Stato Liquidazione annullata");
        }

        private void ElaboraLiquidazione(int liquidazioneId, string note, string allegato, int statoId)
        {
            var cartellaServer = GetUploadFolder(PathLiquidazione, liquidazioneId);

            var filename = Savefile(cartellaServer, allegato);

            var _liquidazione = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId).FirstOrDefault();
            _liquidazione.StatoLiquidazioneId = statoId;
            _liquidazione.Allegato = filename;
            _liquidazione.Note = note;
            _liquidazione.DataLavorazione = DateTime.Now;

            unitOfWork.LiquidazioneRepository.Update(_liquidazione);
            unitOfWork.Save();

            //update list ricerca utente
            Task.Run(() =>
            {
                List<string> _usernames = new List<string>();
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();

                foreach (var item in _liquidazione.LiquidazionePraticheRegionali.Select(x => x.PraticheRegionaliImprese))
                {
                    _usernames.Add(item.UserInserimento);


                    if (item.DipendenteId == null && item.Azienda != null)
                    {
                        _usernames.Add(item.Azienda?.MatricolaInps);
                    }

                    if (item.DipendenteId == null && item.ConsulenteCS != null)
                    {
                        _usernames.Add(item.ConsulenteCS?.CodiceFiscalePIva);
                    }

                    if (item.DipendenteId != null)
                    {
                        _usernames.Add(item.Dipendente?.CodiceFiscale);
                    }

                }

                foreach (var item in _usernames.Distinct())
                {
                    context.Clients.All.onUpdateListRicerca(item);
                }
            });
        }

        public ActionResult DownloadAllegato(int liquidazioneId)
        {
            try
            {
                var _uploadFolder = GetUploadFolder(PathLiquidazione, liquidazioneId);

                var _allegato = unitOfWork.LiquidazioneRepository.Get(xx => xx.LiquidazioneId == liquidazioneId)?.FirstOrDefault();

                if (_allegato == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _allegato.Allegato)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = MimeMapping.GetMimeMapping(_allegato.Allegato);
                return File(Path.Combine(_uploadFolder, _allegato.Allegato), mimeType, "LiquidazioneAllegato." + Path.GetExtension(_allegato.Allegato));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult DownloadSepa(int liquidazioneId)
        {
            try
            {

                SepaProvider sepa = new SepaProvider();

                return File(sepa.SepaStream(liquidazioneId).ToArray(), "application.xml", $"Sepa_{liquidazioneId.ToString().PadLeft(7, '0')}.xml");
                //return Json(new
                //{
                //    isValid = true,
                //    message = "OK",
                //    base64 = sepa.SepaBase64String(liquidazioneId)
                //}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
                //return JsonResultFalse(ex.Message);
            }

        }
    }
}
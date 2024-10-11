using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using EBLIG.DOM;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Providers;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;
using Sediin.MVC.HtmlHelpers;

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
            var _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaDaLiquidareFilter(model)).AsQueryable()
                .OrderBy(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_OrderBy));

            var _result = GeModelWithPaging<LiquidazioneDaLiquidareRicercaViewModel, PraticheRegionaliImprese>(page, _query, model, 10);
            _result.ImportoDaLiquidare = _query.Sum(x => x.ImportoContributoNetto);
            _result.ListaPraticheLiquidazione = ListaPraticheLiquidazione;
            _result.ImportoListaPraticheLiquidazione = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => ListaPraticheLiquidazione.Contains(xx.PraticheRegionaliImpreseId))?.Sum(x => x.ImportoContributoNetto);

            return AjaxView("RicercaDaLiquidareList", _result);
        }

        private Expression<Func<PraticheRegionaliImprese, bool>> RicercaDaLiquidareFilter(LiquidazioneDaLiquidareRicercaModel model)
        {
            if (model == null)
            {
                model = new LiquidazioneDaLiquidareRicercaModel();
            }
            DateTime _d1 = DateTime.Now;
            DateTime? _datastart = null;
            if (!string.IsNullOrWhiteSpace(model.PraticheAziendaRicercaModel_DataInvio))
            {
                DateTime.TryParse(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_DataInvio), out _d1);
                _datastart = new DateTime(_d1.Year, _d1.Month, _d1.Day, 0, 0, 0);
            }

            DateTime? _dataend = null;
            if (!string.IsNullOrWhiteSpace(model.PraticheAziendaRicercaModel_DataInvio))
            {
                _dataend = new DateTime(_d1.Year, _d1.Month, _d1.Day, 23, 59, 59);
            }

            var _liquidatiOinliquidazione = unitOfWork.LiquidazionePraticheRegionaliRepository.Get(xx =>
            xx.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.InLiquidazione
            || xx.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.Liquidata).Select(x => x.PraticheRegionaliImpreseId);

            return x =>
            (x.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata) && (!_liquidatiOinliquidazione.Contains(x.PraticheRegionaliImpreseId))
            && ((model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId != 0 && model.PraticheAziendaRicercaModel_TipoRichiestaId != -1) ? x.TipoRichiestaId == model.PraticheAziendaRicercaModel_TipoRichiestaId : true)
            && (model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 || model.PraticheAziendaRicercaModel_TipoRichiestaId == -1) ? (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 ? x.TipoRichiesta.IsTipoRichiestaDipendente != true : x.TipoRichiesta.IsTipoRichiestaDipendente == true) : true)
            && (model.PraticheAziendaRicercaModel_AziendaId != null ? x.AziendaId == model.PraticheAziendaRicercaModel_AziendaId : true)
            && (model.PraticheAziendaRicercaModel_DataInvio != null ? x.DataInvio != null && (_datastart < x.DataInvio && _dataend > x.DataInvio) : true)
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
        public ActionResult CreaUnicaListaLiquidazione()
        {
            var _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaDaLiquidareFilter(null)).AsQueryable();

            unitOfWork.LiquidazioneRepository.Insert(new Liquidazione
            {
                DataCreazione = DateTime.Now,
                StatoLiquidazioneId = (int)EbligEnums.StatoLiqidazione.InLiquidazione,
                LiquidazionePraticheRegionali = _query.Select(x => new LiquidazionePraticheRegionali
                {
                    PraticheRegionaliImpreseId = x.PraticheRegionaliImpreseId
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

        public ActionResult LiquidazioneRichiesteMailInviatiExcel(int liquidazioneId)
        {
            var _query = from a in unitOfWork.LiquidazionePraticheRegionaliMailInviatiEsitoRepository.Get(x => x.LiquidazioneId == liquidazioneId)
                         select new
                         {
                             a.Email,
                             Inviata = a.Inviata == true ? "Inviata" : "Errore invio",
                             a.Esito
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "LiquidazioniRichiestemailInviate");
        }

        public ActionResult LiquidazioneRichiesteExcel(int liquidazioneId)
        {
            var _query = from a in unitOfWork.LiquidazionePraticheRegionaliRepository.Get(x => x.LiquidazioneId == liquidazioneId)
                         .Select(x => x.PraticheRegionaliImprese)
                         select new
                         {
                             PrestazioneRegionale = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? "Dipendenti" : "Aziende",
                             Stato = a.StatoPratica.Descrizione,
                             TipoRichiesta = a.TipoRichiesta.Descrizione,
                             RagioneSociale = a.Azienda != null ? $"{a.Azienda.RagioneSociale}" : "",
                             MatricolaInps = a.Azienda != null ? $"{a.Azienda.MatricolaInps}" : "",
                             Dipendente = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? (a.Dipendente != null ? $"{a.Dipendente.Nome} {a.Dipendente.Cognome}" : "") : null,
                             DipendenteCodiceFiscale = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? (a.Dipendente != null ? $"{a.Dipendente.CodiceFiscale}" : "") : null,
                             DataRichiesta = a.DataInserimento.ToShortDateString(),
                             DataInvio = a.DataInvio != null ? a.DataInvio.Value.ToShortDateString() : null,
                             Protocollo = a.ProtocolloId,
                             ImportoContributo = a.ImportoContributo,
                             AliquoteIRPEF = a.AliquoteIRPEF,
                             ImportoIRPEF = a.ImportoIRPEF,
                             ImportoRichiesta = a.ImportoContributoNetto,
                             Note = a.Azienda != null && a.Azienda.Copertura != null && a.Azienda.Copertura.FirstOrDefault() != null
                             ? (a.Azienda.Copertura.FirstOrDefault().Coperto == false ? "L'azienda non risulta in regola con i contributi" : "") : ""
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "LiquidazioniRichieste");
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

        [HttpGet]
        public ActionResult MailAvvenutoPagamento(int liquidazioneId)
        {
            //HttpContext context = HttpContext.ApplicationInstance.Context;

            var _bodyaz = RenderTemplate("Liquidazione/AvvenutoPagamentoMail_Azienda", new LiquidazioneAvvenutoPagamentoMail_Azienda
            {
                Ragionesociale = "@Model.Ragionesociale",
                Iban = "@Model.Iban",
                Importo = "@Model.Importo",
                NominativiDipendenti = "@Model.NominativiDipendenti",
                TipoRichiesta = "@Model.TipoRichiesta"
            });

            var _bodydp = RenderTemplate("Liquidazione/AvvenutoPagamentoMail_Dipendente", new LiquidazioneAvvenutoPagamentoMail_Dipendente
            {
                Nominativo = "@Model.Nominativo",
                Importo = "@Model.Importo",
                Ragionesociale = "@Model.Ragionesociale",
                TipoRichiesta = "@Model.TipoRichiesta"
            });

            LiquidazioneIdProvider p = new LiquidazioneIdProvider
            {
                Username = User.Identity.Name,
                Ruolo = GetUserRole(),
                BodyMailDipendente = _bodydp,
                BodyMailAzienda = _bodyaz,
                // CurrentHttpContext = context
            };

            p.OnSendMailLiquidazioneReport += P_OnSendMailLiquidazioneReport;
            p.OnSuccessSendMailLiquidazioneReport += P_OnSuccessSendMailLiquidazioneReport;
            p.OnErrorFileMailLiquidazione += P_OnErrorFileMailLiquidazione;
            Task.Run(() => p.ProcessSendMailLiquidazione(liquidazioneId));

            return JsonResultTrue("Invio mail avvito");
        }

        private void P_OnErrorFileMailLiquidazione(string base64, string tipo, string username, string ruolo)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onReportImportError(base64, username);

            Task.Run(() =>
            {
                try
                {
                    UnitOfWork _unitOfWork = new UnitOfWork();
                    _unitOfWork.LogsRepository.Insert(new Logs
                    {
                        Data = DateTime.Now,
                        Ruolo = ruolo,
                        Username = username,
                        Model = null,
                        ViewDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64)).Replace(Environment.NewLine, "<br/>"),
                        Message = "Errore Import " + tipo,
                        Action = "SendMailLiquidazione"
                    });
                    _unitOfWork.Save();
                }
                catch
                {
                }
            });
        }

        private void P_OnSuccessSendMailLiquidazioneReport(string processoId, string username, string tipoImport, int index, int totale, string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onReportImportStatus(processoId, username, tipoImport, index, totale, message);
        }

        private string P_OnSendMailLiquidazioneReport(LiquidazioneIdProvider.SendMailLiquidazioneEmailResultModel model)
        {
            try
            {
                var _body = model.Body;

                foreach (var item in typeof(LiquidazioneIdProvider.SendMailLiquidazioneEmailResultModel).GetProperties())
                {
                    if (item.Name == nameof(model.NominativiDipendenti))
                    {
                        var _nominatividipendenti = "";
                        if (model.NominativiDipendenti?.Count() > 0)
                        {
                            foreach (var nom in model.NominativiDipendenti)
                            {
                                _nominatividipendenti += $"<li>{nom}</li>";
                            }
                        }

                        _nominatividipendenti = $"<ul>{_nominatividipendenti}</ul>";

                        _body = _body.Replace("@Model." + item.Name, _nominatividipendenti);

                    }
                    else
                    {
                        _body = _body.Replace("@Model." + item.Name, item.GetValue(model)?.ToString());

                    }
                }

                var _b = SendMailAsync(new WebUI.Models.SimpleMailMessage
                {
                    ToEmail = model.Email,
                    ToName = model.Nominativo ?? model.Ragionesociale,
                    Subject = "EBLIG - Avviso liquidazione pratica",
                    Body = _body
                }).Result;

                if (_b)
                {
                    return "";
                }
                else
                {
                    return "Si e verificato un errore invio della mail per: " + model.Email;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


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
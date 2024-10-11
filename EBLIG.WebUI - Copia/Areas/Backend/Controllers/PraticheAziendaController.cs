using DocumentFormat.OpenXml.EMMA;
using EBLIG.DOM;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Models;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using EBLIG.WebUI.ValidationAttributes;
using Microsoft.AspNet.SignalR;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using static EBLIG.WebUI.Areas.Backend.Models.PraticheAzienda_Dipendente_CarenzaMalattia;
using static EBLIG.WebUI.Areas.Backend.Models.PraticheAzienda_Dipendente_Parentela;
using static EBLIG.WebUI.Areas.Backend.Models.PraticheAzienda_IncrementoMantenimentoOccupazione;
using Reflection = Sediin.MVC.HtmlHelpers.Reflection;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class PraticheAziendaController : BaseController
    {
        public string PathPraticheAzienda { get => "PraticheAzienda\\Richiesta\\{0}"; private set { } }

        #region ricerca

        public ActionResult Ricerca()
        {
            PraticheAziendaRicercaModel model = new PraticheAziendaRicercaModel
            {
                TipoRichiesta = GetTipoRichieste(),
                StatoPratica = GetStatoPratica(),
            };

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        public ActionResult Ricerca(PraticheAziendaRicercaModel model, int? page)
        {
            var _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaFilter(model))
                .OrderBy(x => x.DataInvio == null).ThenBy(x => x.DataInvio).ThenBy(x => x.DataInserimento);

            var _confermati = _query?.Where(x => x.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata);

            var _netto = _confermati.Sum(x => x.ImportoContributoNetto);
            var _liquidati = _confermati.Where(c => c.LiquidazionePraticheRegionali.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.Liquidata).Count() > 0)?.Sum(x => x.ImportoContributoNetto);
            var _inliquidazione = _confermati.Where(c => c.LiquidazionePraticheRegionali.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)EbligEnums.StatoLiqidazione.InLiquidazione).Count() > 0)?.Sum(x => x.ImportoContributoNetto);
            var _daliquidare = _netto - (_liquidati + _inliquidazione);

            var _result = GeModelWithPaging<PraticheAziendaRicercaViewModel, PraticheRegionaliImprese>(page, _query, model, 10);
            _result.ImportoRiconoscitoNetto = _netto;
            _result.ImportoDaLiquidare = _daliquidare;
            _result.ImportoInLiquidare = _inliquidazione;
            _result.ImportoLiquidato = _liquidati;
            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(PraticheAziendaRicercaModel model)
        {
            var _query = from a in unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaFilter(model))
                         select new
                         {
                             PrestazioneRegionale = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? "Dipendenti" : "Aziende",
                             Stato = a.StatoPratica.Descrizione,
                             TipoRichiesta = a.TipoRichiesta.Descrizione,
                             RagioneSociale = $"{a.Azienda.RagioneSociale} - {a.Azienda.MatricolaInps}",
                             Dipendete = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? $"{a.Dipendente?.Nome} {a.Dipendente?.Cognome}" : null,
                             DataRichiesta = a.DataInserimento.ToShortDateString(),
                             ImportoContributo = a.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata ? a.ImportoContributo : null,
                             AliquoteIRPEF = a.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata ? a.AliquoteIRPEF : null,
                             ImportoIRPEF = a.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata ? a.ImportoIRPEF : null,
                             ImportoContributoNetto = a.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata ? a.ImportoContributoNetto : null
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "PraticheRegionali");
        }

        private Expression<Func<PraticheRegionaliImprese, bool>> RicercaFilter(PraticheAziendaRicercaModel model)
        {
            //var _dipendentiIdSportello = new List<int>();
            //if (IsUserSportello)
            //{
            //    _dipendentiIdSportello = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.DelegaAttiva == true).Select(x => x.DipendenteId).ToList();
            //}

            //var _aziendeIdSportello = new List<int>();
            //if (IsUserConsulenteCs)
            //{
            //    _aziendeIdSportello = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(x => x.DelegaAttiva == true).Select(x => x.AziendaId).ToList();
            //}

            return x => ((IsUserConsulenteCs ? (x.ConsulenteCSId == (int)GetConsulenteCsId.Value && x.DipendenteId == null) : true)
            && (IsUserAzienda ? (x.AziendaId == (int)GetAziendaId.Value && x.DipendenteId == null) : true)
            && (IsUserDipendente ? (x.DipendenteId == (int)GetDipendenteId.Value && x.SportelloId == null) : true)
            && (IsUserSportello ? (x.SportelloId == (int)GetSportelloId.Value) : true))
            && ((model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId != 0 && model.PraticheAziendaRicercaModel_TipoRichiestaId != -1) ? x.TipoRichiestaId == model.PraticheAziendaRicercaModel_TipoRichiestaId : true)
            && (model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 || model.PraticheAziendaRicercaModel_TipoRichiestaId == -1) ? (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 ? x.TipoRichiesta.IsTipoRichiestaDipendente != true : x.TipoRichiesta.IsTipoRichiestaDipendente == true) : true)
            //&& (model.PraticheAziendaRicercaModel_TipoRichiestaId != null ? x.TipoRichiestaId == model.PraticheAziendaRicercaModel_TipoRichiestaId : true)
            && (model.PraticheAziendaRicercaModel_AziendaId != null ? x.AziendaId == model.PraticheAziendaRicercaModel_AziendaId : true)
            && (model.PraticheAziendaRicercaModel_DipendenteId != null ? x.DipendenteId == model.PraticheAziendaRicercaModel_DipendenteId : true)
            && (model.PraticheAziendaRicercaModel_ProtocolloId != null ? x.ProtocolloId.Contains(model.PraticheAziendaRicercaModel_ProtocolloId) : true)
            && (model.PraticheAziendaRicercaModel_StatoPraticaId != null ? (model.PraticheAziendaRicercaModel_StatoPraticaId == -1 ? x.StatoPraticaId == (int)EbligEnums.StatoPratica.Inviata || x.StatoPraticaId == (int)EbligEnums.StatoPratica.InviataRevisionata : x.StatoPraticaId == model.PraticheAziendaRicercaModel_StatoPraticaId) : true));

        }

        #endregion
        public ActionResult NuovaRichiestaSportello()
        {
            if (!IsUserSportello)
            {
                throw new Exception("Richiesta non valida");
            }
            return AjaxView();
        }

        [HttpPost]
        public ActionResult NuovaRichiestaSportello(PraticheAziendaNuovaRichiestaSportello model)
        {
            try
            {
                if (!IsUserSportello)
                {
                    throw new Exception("Richiesta non valida");
                }

                if (unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == model.PraticheAziendaNuovaRichiesta_DipendenteId && x.DataCessazione == null).Count() == 0)
                {
                    throw new Exception("E' necessario associare un'azienda. Clicca nel menu' laterale sulla voce \"Aziende associate\"");
                }

                return NuovaRichiesta(new PraticheAziendaNuovaRichiesta
                {
                    PraticheAziendaNuovaRichiesta_DipendenteId = model.PraticheAziendaNuovaRichiesta_DipendenteId,
                    PraticheAziendaNuovaRichiesta_NominativoDipendente = model.PraticheAziendaNuovaRichiesta_NominativoDipendente
                });

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult NuovaRichiestaAltro()
        {
            return NuovaRichiesta(new PraticheAziendaNuovaRichiesta
            {
            });
        }

        private ActionResult NuovaRichiesta(PraticheAziendaNuovaRichiesta model = null)
        {
            try
            {
                if (IsUserAdmin)
                {
                    throw new Exception("Amministratore EBLIG non po inserire nuove richiesta");
                }

                if (IsUserDipendente)
                {
                    var _dipendenteId = this.GetDipendenteId.Value;
                    if (unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == _dipendenteId && x.DataCessazione == null).Count() == 0)
                    {
                        throw new Exception("E' necessario associare un'azienda. Clicca nel menu' laterale sulla voce \"Aziende associate\"");
                    }
                    model.PraticheAziendaNuovaRichiesta_DipendenteId = _dipendenteId;
                }

                if (model == null)
                {
                    model = new PraticheAziendaNuovaRichiesta();
                }

                model.TipoRichiesta = GetTipoRichieste().Where(a => a.AbilitatoNuovaRichiesta == true);

                if (IsUserAzienda)
                {
                    model.PraticheAziendaNuovaRichiesta_AziendaId = GetAziendaId;
                }

                return AjaxView("NuovaRichiesta", model);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;

                }
                return AjaxView("Error", new HandleErrorInfo(ex, "PraticheAziendaController", "NuovaRichiesta"));
            }
        }

        [HttpPost]
        public ActionResult CreateNuovaRichiesta(PraticheAziendaNuovaRichiesta model)
        {
            try
            {
                if (IsUserAdmin)
                {
                    throw new Exception("Amministratore EBLIG non po inserire nuove richiesta");
                }

                if (!IsUserSportello)
                {
                    ModelState.Remove("PraticheAziendaNuovaRichiesta_DipendenteId");
                }

                if (IsUserAzienda)
                {
                    model.PraticheAziendaNuovaRichiesta_AziendaId = GetAziendaId;
                    ModelState.Remove("PraticheAziendaNuovaRichiesta_AziendaId");
                    ModelState.Remove("PraticheAziendaNuovaRichiesta_RagioneSociale");
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.AbilitatoNuovaRichiesta == true && xx.TipoRichiestaId == model.PraticheAziendaNuovaRichiesta_TipoRichiestaId);

                if (_tiporichiesta == null)
                {
                    throw new Exception("Tipo richiesta non valida");
                }

                //se azienda, settare aziendaid
                var _aziendaId = model.PraticheAziendaNuovaRichiesta_AziendaId.Value;
                if (IsUserAzienda)
                {
                    _aziendaId = GetAziendaId.Value;
                }

                //se dipendente, setare dipendenteId
                int? _dipendenteId = null;
                if (IsUserDipendente)
                {
                    _dipendenteId = GetDipendenteId.Value;
                }

                if (IsUserSportello)
                {
                    _dipendenteId = model.PraticheAziendaNuovaRichiesta_DipendenteId;

                    var _result = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == _dipendenteId).FirstOrDefault();
                    if (_result.SportelloId != GetSportelloId.Value)
                    {
                        throw new Exception("Dipendente non e associato alla sua Utenza");
                    }
                }

                if (IsUserConsulenteCs)
                {
                    var _result = unitOfWork.AziendaRepository.Get(x => x.AziendaId == _aziendaId).FirstOrDefault();
                    if (_result.ConsulenteCSId != GetConsulenteCsId.Value)
                    {
                        throw new Exception("Azienda non e associata alla sua Utenza");
                    }
                }

                VerificaMaxRichiesteAzienda(_tiporichiesta, _aziendaId, _dipendenteId);

                //contaner di base
                PraticheAziendaContainer outModel = new PraticheAziendaContainer
                {
                    TipoRichiestaId = model.PraticheAziendaNuovaRichiesta_TipoRichiestaId.Value,
                    AziendaId = _aziendaId,
                    DipendenteId = _dipendenteId,
                    View = _tiporichiesta.View,
                    DescrizioneStato = "Bozza",
                    DescrizioneTipoRichiesta = _tiporichiesta.Descrizione,
                    NoteTipoRichiesta = _tiporichiesta.Note,
                    IbanAziendaRequired = _tiporichiesta.IbanAziendaRequired.GetValueOrDefault(),
                    IbanDipendenteRequired = _tiporichiesta.IbanDipendenteRequired.GetValueOrDefault(),
                };

                var _dataModel = CreateInstance(_tiporichiesta.Classe);
                SetProperty(_dataModel, "AziendaId", outModel.AziendaId);
                SetProperty(_dataModel, "DipendenteId", outModel.DipendenteId);
                SetProperty(_dataModel, "TipoRichiestaId", _tiporichiesta.TipoRichiestaId);
                SetProperty(_dataModel, "TipoRichiesta", _tiporichiesta);

                SetProperty(_dataModel, "CodiceFiscale", IsUserDipendente ? User.Identity.Name : unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == _dipendenteId).FirstOrDefault()?.CodiceFiscale);

                foreach (var item in _dataModel.GetType().GetProperties())
                {
                    foreach (var _attributes in item.GetCustomAttributes(true))
                    {
                        if (_attributes is VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)
                        {
                            var _attribute = ((VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)_attributes);

                            if (item.Name == _attribute.NomeCampo)
                            {
                                var _v = item.GetValue(_dataModel);

                                if (PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(_aziendaId, _tiporichiesta.TipoRichiestaId, _v?.ToString(), 0, _attribute.NomeCampo, _attribute.Unica))
                                {
                                    // _errorsUnivocoCodiceFiscale.Add($"Per il Codice Fiscale <strong>{_v}</strong> e già stato fatto una richiesta");
                                    throw new Exception($"Per il Codice Fiscale {_v} e già stata presentata una richiesta");
                                    //throw new Exception(_attribute.ErrorMessage);
                                }
                            }
                        }
                    }
                }

                outModel.DataModel = _dataModel;

                return AjaxView("PraticheAziendaContainer", outModel);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;
                }
                return AjaxView("Error", new HandleErrorInfo(ex, "PraticheAziendaController", "Apri"));
                //return JsonResultFalse(ex.Message);
            }
        }

        private void VerificaMaxRichiesteAzienda(TipoRichiesta tiporichiesta, int aziendaId, int? dipendenteId = null, int? richiestaId = null)
        {
            try
            {
                if (tiporichiesta.MaxRichiesteAnno.HasValue)
                {
                    if (tiporichiesta.MaxRichiesteAnno == 0)
                    {
                        throw new Exception("Non e possibile effetuare questo tipo di richiesta");
                    }

                    //verificare l'univocità della richiesta, per l'anno corrente
                    //dovrà essere inserita massimo N richieste.
                    var _richieste = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                    (richiestaId.HasValue ? xx.PraticheRegionaliImpreseId != richiestaId : true)
                    && ((dipendenteId.HasValue ? xx.DipendenteId == dipendenteId : true)
                    && xx.AziendaId == aziendaId
                    && xx.TipoRichiestaId == tiporichiesta.TipoRichiestaId
                    && xx.StatoPraticaId != (int)EbligEnums.StatoPratica.Annullata));

                    if (_richieste != null)
                    {
                        if (_richieste.Count() > 0 && (_richieste.Count() >= tiporichiesta.MaxRichiesteAnno.GetValueOrDefault()))
                        {
                            throw new Exception("Per questo tipo di richiesta, Azienda ha già raggiunto il limite massimo");
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ApriRichiesta(int id)
        {
            try
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == id).FirstOrDefault();

                if (_richiesta == null)
                {
                    throw new Exception("Richiesta non trovata");
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.TipoRichiestaId == _richiesta.TipoRichiestaId);

                if (_tiporichiesta == null)
                {
                    throw new Exception("Tipo richiesta non valida");
                }

                #region check se la richiesta e editabile

                var readOnly = _richiesta?.StatoPratica.ReadOnly;

                if (!readOnly.GetValueOrDefault())
                {
                    if (_richiesta.StatoPraticaId == (int)EbligEnums.StatoPratica.Bozza || _richiesta.StatoPraticaId == (int)EbligEnums.StatoPratica.Revisione)
                    {
                        readOnly = IsUserAdmin;
                    }
                }


                #endregion

                var _dataModel = CreateModelDatiRichiesta(_tiporichiesta, _richiesta.DatiPratica);

                var _childClass = CreateModelDatiRichiestaChildClass(_tiporichiesta, _richiesta.DatiPratica, _richiesta.ChildClassRowCount);

                SetProperty(_dataModel, "ChildClass", _childClass);

                SetProvicia(_dataModel, "ProvinciaId", "Provincia");
                SetRegione(_dataModel, "RegioneId", "Regione");
                SetComune(_dataModel, "ComuneId", "Comune");
                SetLocalita(_dataModel, "LocalitaId", "Localita");

                SetProperty(_dataModel, "Iban", _richiesta.Iban);
                SetProperty(_dataModel, "StatoPraticaId", _richiesta.StatoPraticaId);
                SetProperty(_dataModel, "RichiestaId", _richiesta.PraticheRegionaliImpreseId);
                SetProperty(_dataModel, "DipendenteId", _richiesta.DipendenteId);
                SetProperty(_dataModel, "AziendaId", _richiesta.AziendaId);
                SetProperty(_dataModel, "TipoRichiestaId", _tiporichiesta.TipoRichiestaId);
                SetProperty(_dataModel, "TipoRichiesta", _tiporichiesta);
                SetProperty(_dataModel, "ReadOnly", readOnly);

                SetProperty(_dataModel, "AliquoteIRPEF", _richiesta.AliquoteIRPEF);
                SetProperty(_dataModel, "ImportoIRPEF", _richiesta.ImportoIRPEF);
                SetProperty(_dataModel, "ImportoContributo", _richiesta.ImportoContributo);
                SetProperty(_dataModel, "ImportoContributoNetto", _richiesta.ImportoContributoNetto);

                var _liquidataOinLiquidazione = _richiesta.LiquidazionePraticheRegionali?.Where(x => x.Liquidazione.StatoLiquidazioneId != (int)EbligEnums.StatoLiqidazione.Annullata)?.Count() > 0;

                var _descrizioneStato = _richiesta?.StatoPratica?.Descrizione;

                //contaner di base
                PraticheAziendaContainer outModel = new PraticheAziendaContainer
                {
                    AziendaId = _richiesta.AziendaId,
                    RichiestaId = _richiesta.PraticheRegionaliImpreseId,
                    TipoRichiestaId = _richiesta.TipoRichiestaId,
                    StatoId = _richiesta.StatoPraticaId,
                    ProtocolloId = _richiesta.ProtocolloId,
                    DataModel = _dataModel,
                    View = _tiporichiesta.View,
                    DescrizioneStato = _descrizioneStato,
                    DescrizioneTipoRichiesta = _tiporichiesta.Descrizione,
                    NoteTipoRichiesta = _tiporichiesta.Note,
                    IbanAziendaRequired = _tiporichiesta.IbanAziendaRequired.GetValueOrDefault(),
                    IbanDipendenteRequired = _tiporichiesta.IbanDipendenteRequired.GetValueOrDefault(),
                    ReadOnly = readOnly,
                    StoricoStatoPratica = _richiesta.StatoPraticaStorico,
                    DipendenteId = _richiesta.DipendenteId,
                    ChildClassRowCount = _richiesta.ChildClassRowCount,
                    AliquoteIRPEF = _richiesta.AliquoteIRPEF,
                    ImportoContributo = _richiesta.ImportoContributo,
                    ImportoContributoNetto = _richiesta.ImportoContributoNetto,
                    ImportoIRPEF = _richiesta.ImportoIRPEF,
                    Iban = _richiesta.Iban,
                    LiquidataOinLiquidazione = _liquidataOinLiquidazione,
                    PraticheRegionaliImprese = _richiesta,
                };

                return AjaxView("PraticheAziendaContainer", outModel);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;
                }
                return AjaxView("Error", new HandleErrorInfo(ex, "PraticheAziendaController", "Apri"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvaRichiesta(PraticheAziendaContainer container, PraticheAziendaAllegati upload, FormCollection form) //, int iichiestaId, int aziendaId, int tipoRichiestaId)
        {
            try
            {
                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.TipoRichiestaId == container.TipoRichiestaId);

                if (_tiporichiesta == null)
                {
                    throw new Exception("Tipo richiesta non valida");
                }

                //se azienda, settare aziendaid
                var _aziendaId = container.AziendaId;

                if (IsUserAzienda)
                {
                    _aziendaId = GetAziendaId.Value;
                }

                //se dipendente, setare dipendenteId
                int? _dipendenteId = null;
                if (IsUserDipendente)
                {
                    _dipendenteId = GetDipendenteId.Value;
                }

                int? _consulenteId = null;
                if (IsUserConsulenteCs)
                {
                    _consulenteId = GetConsulenteCsId.Value;
                }

                int? _sportelloId = null;
                if (IsUserSportello)
                {
                    _dipendenteId = container.DipendenteId;
                    _sportelloId = GetSportelloId.Value;
                }

                VerificaMaxRichiesteAzienda(_tiporichiesta, _aziendaId, _dipendenteId, container.RichiestaId);

                //get pratica se gia esiste in tb PraticheRegionaliImpreseRepository
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == container.RichiestaId).FirstOrDefault();

                //check stato richiesta
                if (_richiesta != null)
                {
                    if (_richiesta.StatoPraticaId == (int)EbligEnums.StatoPratica.Annullata)
                    {
                        throw new Exception("Richiesta non modificabile");
                    }
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                //crea modello
                var _DatiPratica = CreateInstance(_tiporichiesta.Classe);

                //setta valori del model
                foreach (var item in _DatiPratica.GetType().GetProperties())
                {
                    Reflection.SetValue(_DatiPratica, item.Name, form[item.Name]);
                }

                //modello child
                List<string> _errorsEventi = null;

                //errors UnivocoCodiceFiscale
                List<string> _errorsUnivocoCodiceFiscale = new List<string>();

                Dictionary<string, string> _dicDatiPraticaEventi = new Dictionary<string, string>();

                if (container.ChildClassRowCount > 0)
                {
                    for (int i = 0; i < container.ChildClassRowCount; i++)
                    {
                        var _DatiPraticaEventiModel = CreateInstance(_tiporichiesta.ChildClass);

                        foreach (var item in _DatiPraticaEventiModel.GetType().GetProperties())
                        {
                            var _n = "ChildClass" + "[" + i + "]." + item.Name;
                            var _v = form[_n];

                            //UnivocoCodiceFiscale
                            if (container.Azione == EbligEnums.AzioniPratica.Invia.ToString()
                                || container.Azione == EbligEnums.AzioniPratica.InviaRevisionata.ToString()
                                || container.Azione == EbligEnums.AzioniPratica.Conferma.ToString())
                            {
                                foreach (var _attributes in item.GetCustomAttributes(true))
                                {
                                    if (_attributes is VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)
                                    {
                                        var _attribute = ((VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)_attributes);

                                        if (item.Name == _attribute.NomeCampo)
                                        {
                                            if (PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(_aziendaId, _tiporichiesta.TipoRichiestaId, _v?.ToString(), _richiesta != null ? _richiesta.PraticheRegionaliImpreseId : 0, _attribute.NomeCampo, _attribute.Unica))
                                            {
                                                _errorsUnivocoCodiceFiscale.Add($"Per il Codice Fiscale <strong>{_v}</strong> e già stata presentata una richiesta");
                                                //   throw new Exception($"Per il Codice Fiscale {_v} e già stato fatto una richiesta");
                                                //throw new Exception(_attribute.ErrorMessage);
                                            }
                                        }
                                    }
                                }
                            }

                            _dicDatiPraticaEventi.Add(_n, _v);
                            Reflection.SetValue(_DatiPraticaEventiModel, item.Name, _v);
                        }

                        _errorsEventi = container.Azione == EbligEnums.AzioniPratica.Bozza.ToString()
                            || container.Azione == EbligEnums.AzioniPratica.BozzaRevisionata.ToString()
                            ? null : IsValidModel(new object[] { _DatiPraticaEventiModel });
                    }
                }

                //non validare la request se viene inviata come Azione.Bozza 
                List<string> _errors = container.Azione == EbligEnums.AzioniPratica.Bozza.ToString()
                    || container.Azione == EbligEnums.AzioniPratica.BozzaRevisionata.ToString()
                    ? null : IsValidModel(new object[] { _DatiPratica, upload });

                if (_errorsUnivocoCodiceFiscale?.Count() > 0)
                {
                    if (_errors == null)
                    {
                        _errors = new List<string>();
                    }

                    _errors.AddRange(_errorsUnivocoCodiceFiscale);
                }

                if (_errorsEventi?.Count() > 0)
                {
                    if (_errors == null)
                    {
                        _errors = new List<string>();
                    }

                    _errors.AddRange(_errorsEventi);
                }

                if (_errors?.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_errors));
                }

                decimal? _importoContributo = 0;
                bool? _hashContributoColumn = null;
                var _datiPratica = CreateDatiPratica(_DatiPratica, ref _importoContributo, ref _hashContributoColumn);

                if (!_hashContributoColumn.GetValueOrDefault())
                {
                    _importoContributo = _tiporichiesta.Contributo;
                }

                if (_importoContributo > _tiporichiesta.Contributo)
                {
                    _importoContributo = _tiporichiesta.Contributo;
                }

                //calcola ipref
                var _aliquoteIRPEF = _tiporichiesta.AliquoteIRPEF.GetValueOrDefault();

                var _importoIREF = _importoContributo / 100 * _aliquoteIRPEF;

                var _importoContributoNetto = _importoContributo - _importoIREF;

                //allegati da eliminare da file system dopo il salvataggio, se pratica esiste
                List<string> _filesToDelete = new List<string>();

                //cancella dati pratica
                if (_richiesta != null)// && container.Azione != EbligEnums.AzioniPratica.Conferma.ToString())
                {
                    if (_richiesta.DatiPratica?.Count() > 0)
                    {
                        foreach (var item in _richiesta.DatiPratica.ToList())
                        {
                            unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Delete(item.PraticheRegionaliImpreseDatiPraticaId);
                        }
                    }

                    //allegati da eliminari
                    var _praticheRegionaliImpreseAllegatiId = upload?.File?.Select(x => x.PraticheRegionaliImpreseAllegatiId);

                    var _allegatidaeliminari = _richiesta.Allegati?.Where(d => !_praticheRegionaliImpreseAllegatiId.Contains(d.PraticheRegionaliImpreseAllegatiId));

                    if (_allegatidaeliminari != null)// && _allegatidaeliminari?.Count() > 0)
                    {
                        try
                        {
                            foreach (var item in _allegatidaeliminari?.ToList())
                            {
                                _filesToDelete.Add(item.Filename);
                                unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Delete(item.PraticheRegionaliImpreseAllegatiId);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                //setta statoid della pratica
                var _statoPraticaId = _richiesta != null ? _richiesta.StatoPraticaId : 1;

                if (container.Azione == EbligEnums.AzioniPratica.Bozza.ToString())
                {
                    _statoPraticaId = (int)EbligEnums.StatoPratica.Bozza;
                }

                //imposta data invio
                DateTime? _dataInvio = null;
                if (_richiesta != null)
                {
                    _dataInvio = _richiesta.DataInvio;
                }

                string _usernameInvio = _richiesta != null ? _richiesta.UsernameInvio : null;

                string _ProtocolloId = _richiesta?.ProtocolloId;

                if (container.Azione == EbligEnums.AzioniPratica.Invia.ToString())
                {
                    VerificaCoperturaAzienda(_aziendaId);

                    _dataInvio = DateTime.Now;
                    _usernameInvio = User.Identity.Name;

                    _statoPraticaId = (int)EbligEnums.StatoPratica.Inviata;
                    if (string.IsNullOrWhiteSpace(_ProtocolloId))
                    {
                        _ProtocolloId = $"{DateTime.Now.Year}{DateTime.Now.Month}.{_richiesta.PraticheRegionaliImpreseId.ToString().PadLeft(7, '0')}";
                    }
                }

                if (container.Azione == EbligEnums.AzioniPratica.InviaRevisionata.ToString())
                {
                    VerificaCoperturaAzienda(_aziendaId);

                    _statoPraticaId = (int)EbligEnums.StatoPratica.InviataRevisionata;
                }


                //imposta data conferma
                DateTime? _dataConferma = null;
                if (_richiesta != null)
                {
                    _dataConferma = _richiesta.DataConferma;
                }

                string _usernameConferma = _richiesta != null ? _richiesta.UsernameConferma : null;

                if (container.Azione == EbligEnums.AzioniPratica.Conferma.ToString())
                {
                    _dataConferma = DateTime.Now;
                    _usernameConferma = User.Identity.Name;

                    VerificaCoperturaAzienda(_aziendaId);

                    _statoPraticaId = (int)EbligEnums.StatoPratica.Confermata;
                }

                //inserimento/aggiornamento della pratica
                PraticheRegionaliImprese praticheRegionaliImprese = new PraticheRegionaliImprese
                {
                    PraticheRegionaliImpreseId = container.RichiestaId,
                    AziendaId = _richiesta != null ? _richiesta.AziendaId : _aziendaId,
                    DipendenteId = _richiesta != null ? _richiesta.DipendenteId : _dipendenteId,
                    SportelloId = _richiesta != null ? _richiesta.SportelloId : _sportelloId,
                    //unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == _dipendenteId).FirstOrDefault()?.SportelloId, // User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()) ? GetConsulenteCsId : (_richiesta != null ? _richiesta.ConsulenteCSId : null),
                    ConsulenteCSId = _richiesta != null ? _richiesta.ConsulenteCSId : _consulenteId,
                    DataInserimento = _richiesta != null ? _richiesta.DataInserimento : DateTime.Now,
                    DataInvio = _dataInvio,
                    StatoPraticaId = _statoPraticaId,
                    TipoRichiestaId = _richiesta != null ? _richiesta.TipoRichiestaId : container.TipoRichiestaId,
                    UserInserimento = _richiesta != null ? _richiesta.UserInserimento : User.Identity.Name,
                    RuoloUserInserimento = _richiesta != null ? _richiesta.RuoloUserInserimento : GetUserRole(),
                    ProtocolloId = _ProtocolloId,
                    ImportoContributo = _importoContributo,
                    ImportoIRPEF = _importoIREF,
                    ImportoContributoNetto = _importoContributoNetto,
                    AliquoteIRPEF = _aliquoteIRPEF,
                    ChildClassRowCount = container.ChildClassRowCount,
                    Iban = container.Iban,
                    DataConferma = _dataConferma,
                    UsernameInvio = _usernameInvio,
                    UsernameConferma = _usernameConferma,
                };

                unitOfWork.PraticheRegionaliImpreseRepository.InsertOrUpdate(praticheRegionaliImprese);
                //commit se nuova
                if (container.RichiestaId == 0)
                {
                    unitOfWork.Save(false);
                }

                //storico stato pratica
                var _storicoStatoPratica = _richiesta?.StatoPraticaStorico?.OrderByDescending(d => d.PraticheRegionaliImpreseStatoPraticaStoricoId).FirstOrDefault();

                var _insertStorico = _storicoStatoPratica == null || _storicoStatoPratica.StatoPraticaId != _statoPraticaId;

                if (_insertStorico)
                {
                    PraticheRegionaliImpreseStatoPraticaStorico praticheRegionaliImpreseStatoPratica = new PraticheRegionaliImpreseStatoPraticaStorico
                    {
                        StatoPraticaId = _statoPraticaId,
                        DataInserimento = DateTime.Now,
                        PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId,
                        UserName = User.Identity.Name,
                        UserRuolo = GetUserRole()
                    };

                    unitOfWork.PraticheRegionaliImpreseStatoPraticaStoricoRepository.Insert(praticheRegionaliImpreseStatoPratica);
                }

                //insert dati pratica

                if (_datiPratica?.Count() > 0)
                {
                    foreach (var item in _datiPratica)
                    {
                        item.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                        unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Insert(item);
                    }
                }

                if (_dicDatiPraticaEventi?.Count() > 0)
                {
                    foreach (var item in _dicDatiPraticaEventi)
                    {
                        PraticheRegionaliImpreseDatiPratica _PraticheRegionaliImpreseDatiPratica = new PraticheRegionaliImpreseDatiPratica();
                        _PraticheRegionaliImpreseDatiPratica.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                        _PraticheRegionaliImpreseDatiPratica.Nome = item.Key;
                        _PraticheRegionaliImpreseDatiPratica.Valore = item.Value;
                        unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Insert(_PraticheRegionaliImpreseDatiPratica);
                    }
                }

                //insert allegati pratica
                var _allegati = CreateAllegati(upload, praticheRegionaliImprese.PraticheRegionaliImpreseId);

                if (_allegati?.Count() > 0)
                {
                    foreach (var item in _allegati)
                    {
                        if (item.TipoRichiestaAllegatiId == 0)
                        {
                            continue;
                        }

                        item.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                        unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Insert(item);
                    }
                }

                //commit tutto
                unitOfWork.Save(false);

                //aggiorna lista ricerca
                UpdateListRicerca(praticheRegionaliImprese.PraticheRegionaliImpreseId);

                //avvisa admin che ce una nuova richiesta
                if (!User.IsInRole(IdentityHelper.Roles.Admin.ToString()) && ((_statoPraticaId == (int)EbligEnums.StatoPratica.Inviata || _statoPraticaId == (int)EbligEnums.StatoPratica.InviataRevisionata)))
                {
                    AvvisaAdmin($"<strong>Una nuova richiesta:</strong><br/>{(_richiesta.TipoRichiesta?.IsTipoRichiestaDipendente == true ? "Prestazioni Regionali Dipendenti" : "Prestazioni Regionali Azienda")}<br/>{_richiesta.TipoRichiesta?.Descrizione} {_richiesta.TipoRichiesta?.Anno}");
                }

                if (_statoPraticaId == (int)EbligEnums.StatoPratica.Confermata)
                {
                    ConfermaRichiestaMail(_richiesta);

                    AvvisaUtente(_richiesta.PraticheRegionaliImpreseId, "<strong>Informazione:</strong><br/>La sua Richiesta " + _richiesta.TipoRichiesta.Descrizione + " e stata confermata con Protocollo: " + _richiesta.ProtocolloId);
                }

                //cancella allegati eliminati
                Task.Run(() => DeleteFiles(_filesToDelete, praticheRegionaliImprese.PraticheRegionaliImpreseId));

                return JsonResultTrue(praticheRegionaliImprese.PraticheRegionaliImpreseId, "Richiesta " + (container.RichiestaId == 0 ? "inserita" : "aggiornata"));
            }
            //catch(DbEntityValidationException dbex)
            //{
            //    foreach (var eve in dbex.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }

            //    return JsonResultFalse(container.RichiestaId, dbex.Message);
            //}
            catch (Exception ex)
            {
                throw;
                //return JsonResultFalse(container.RichiestaId, ex.Message);
            }
        }

        public ActionResult AnagraficaDipendente(int? dipendenteId, string iban, bool? ibanRequired, bool? readOnly)
        {
            var model = Reflection.CreateModel<DipendentePrestazioniRegionaliViewModel>(unitOfWork.DipendenteRepository.Get(xx => xx.DipendenteId == dipendenteId).FirstOrDefault());

            if (!string.IsNullOrEmpty(iban))
            {
                model.Iban = iban;
            }

            model.ReadOnly = readOnly;
            model.IbanRequired = ibanRequired.GetValueOrDefault();

            return AjaxView("AnagraficaDipendente", model);
        }

        public ActionResult AnagraficaAzienda(int? aziendaId, string iban, bool? ibanRequired, bool? readOnly)
        {
            var _azienda = unitOfWork.AziendaRepository.Get(xx => xx.AziendaId == aziendaId).FirstOrDefault();

            var model = Reflection.CreateModel<AziendaPrestazioniRegionaliViewModel>(_azienda);

            if (!string.IsNullOrEmpty(iban))
            {
                model.Iban = iban;
            }

            model.ReadOnly = readOnly;
            model.IbanRequired = ibanRequired.GetValueOrDefault();
            model.AziendaCoperta = true;

            if (_azienda.Copertura != null && _azienda.Copertura.FirstOrDefault()?.Coperto == false)
            {
                model.AziendaCoperta = false;
            }

            return AjaxView("AnagraficaAzienda", model);
        }

        public ActionResult AllegatiRichiesta(int? tipoRichiestaId, int? richiestaId, bool? readOnly)
        {
            PraticheAziendaAllegati model = new PraticheAziendaAllegati();

            if (richiestaId.HasValue)
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                model.RichiestaId = _richiesta.PraticheRegionaliImpreseId;
                model.RichiestaAllegati = _richiesta?.Allegati?.ToList();
                model.ReadOnly = readOnly;
            }

            model.TipoRichiestaAllegati = GetTipoRichiestaAllegati(tipoRichiestaId.Value);
            return PartialView("AllegatiRichiesta", model);
        }

        public ActionResult DownloadAllegato(int richiestaId, string allegato)
        {
            try
            {
                var _uploadFolder = GetUploadFolder(PathPraticheAzienda, richiestaId);

                var _allegato = unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId && xx.Filename.StartsWith(allegato))?.FirstOrDefault();

                if (_allegato == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _allegato.Filename)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = System.Web.MimeMapping.GetMimeMapping(_allegato.Filename);
                return File(Path.Combine(_uploadFolder, _allegato.Filename), mimeType, _allegato.FilenameOriginale);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public string GetNoteTipoRichiesta(int? id = null)
        {
            if (id.GetValueOrDefault() == 0)
            {
                return null;
            }
            return GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == id)?.Note;
        }

        public ActionResult IncrementoMantenimentoOccupazione_RichiedenteNuovo(int aziendaId, int tipoRichiestaId, string modalId, int? richiestaId = null, IEnumerable<Richiedente> childClass = null)
        {
            Richiedente model = new Richiedente
            {
                RichiestaId = richiestaId,
                AziendaId = aziendaId,
                TipoRichiestaId = tipoRichiestaId,
                ModalId = modalId,
                Richiedenti = childClass
            };

            return PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/IncrementoMantenimentoOccupazione_RichiedenteNuovo.cshtml", model);
        }

        [HttpPost]
        public ActionResult IncrementoMantenimentoOccupazione_RichiedenteNuovo_Add(Richiedente model, IEnumerable<Richiedente> childClass = null)
        {
            try
            {
                var _error = IsValidModel(new object[] { model });

                if (_error.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_error));
                }

                RichiedentiViewModel _model = new RichiedentiViewModel();
                _model.ChildClass = new List<Richiedente>();

                if (childClass != null)
                {
                    _model.ChildClass = childClass.ToList();
                }

                if (_model.ChildClass.Count() > 0 && _model.ChildClass.FirstOrDefault(c => c.CodiceFiscale?.ToUpper() == model.CodiceFiscale.ToUpper()) != null)
                {
                    throw new Exception("Codice Fiscale e già stato aggiunto");
                }

                model.CodiceFiscale = model.CodiceFiscale?.ToUpper();

                _model.ChildClass.Add(model);

                return PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/IncrementoMantenimentoOccupazione_RichiedentiLista.cshtml", _model);

            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult IncrementoMantenimentoOccupazione_RichiedenteRimuovo(string codiceFIscale, IEnumerable<Richiedente> childClass)
        {
            var model = childClass.ToList();

            model.Remove(childClass.FirstOrDefault(x => x.CodiceFiscale?.ToUpper() == codiceFIscale.ToUpper()));

            RichiedentiViewModel _model = new RichiedentiViewModel();

            _model.ChildClass = model;

            return PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/IncrementoMantenimentoOccupazione_RichiedentiLista.cshtml", _model);
        }


        [ChildActionOnly]
        public ActionResult Azioni(int? richiestaId, int? tipoRichiestaId, int? statoId, bool? liquidataOinLiquidazione)
        {
            var _role = GetUserRole();
            PraticheAziendaAzioni model = new PraticheAziendaAzioni();
            model.TipoRichiestaId = tipoRichiestaId;
            model.RichiestaId = richiestaId;
            model.StatoId = statoId;
            model.Azioni = unitOfWork.AzioniPraticaRepository.Get(xx => xx.StatoPraticaId == statoId && xx.TipoRichiestaId == tipoRichiestaId);
            model.AzioniRuolo = unitOfWork.AzioniRuoloRepository.Get(xx => xx.Ruolo == _role && xx.StatoPraticaId == statoId);
            model.LiquidataOinLiquidazione = liquidataOinLiquidazione;

            return PartialView(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult Revisione(int richiestaId)
        {
            PraticheAziendaRevisione_Annulla model = new PraticheAziendaRevisione_Annulla
            {
                RichiestaId = richiestaId,
                StatoPratica = EbligEnums.StatoPratica.Revisione,
                Motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.StatoPraticaId == (int)EbligEnums.StatoPratica.Revisione)
            };

            return PartialView("RevisioneAnnulla", model);
        }

        [HttpPost]
        public ActionResult Annulla(int richiestaId)
        {
            PraticheAziendaRevisione_Annulla model = new PraticheAziendaRevisione_Annulla
            {
                RichiestaId = richiestaId,
                StatoPratica = EbligEnums.StatoPratica.Annullata,
                Motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.StatoPraticaId == (int)EbligEnums.StatoPratica.Annullata)
            };

            return PartialView("RevisioneAnnulla", model);
        }

        [HttpPost]
        //[AuthorizeAdmin]
        public ActionResult Revisione_Annulla(PraticheAziendaRevisione_Annulla model)
        {
            try
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == model.RichiestaId).FirstOrDefault();

                if (_richiesta.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata)
                {
                    throw new Exception("Richiesta non pò essere messa nello nuovo stato");
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                //update stato richiesta
                _richiesta.StatoPraticaId = (int)Enum.Parse(typeof(EbligEnums.StatoPratica), model.StatoPratica.ToString(), true);
                var _statoEnum = (EbligEnums.StatoPratica)_richiesta.StatoPraticaId;

                unitOfWork.PraticheRegionaliImpreseRepository.Update(_richiesta);

                //inserimento in storico stato
                PraticheRegionaliImpreseStatoPraticaStorico praticheRegionaliImpreseStatoPratica = new PraticheRegionaliImpreseStatoPraticaStorico
                {
                    StatoPraticaId = _richiesta.StatoPraticaId,
                    DataInserimento = DateTime.Now,
                    PraticheRegionaliImpreseId = _richiesta.PraticheRegionaliImpreseId,
                    UserName = User.Identity.Name,
                    UserRuolo = GetUserRole(),
                    Note = model.Note,
                    MotivazioniId = model.MotivazioneId
                };

                unitOfWork.PraticheRegionaliImpreseStatoPraticaStoricoRepository.Insert(praticheRegionaliImpreseStatoPratica);

                unitOfWork.Save(false);

                //invia mail
                var _email = GetEmailAddressFromRichiesta(_richiesta);
                var _nome = GetNominativoFromRichiesta(_richiesta);
                var motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.MotivazioniId == model.MotivazioneId)?.FirstOrDefault();

                var _body = RenderTemplate("PraticheAzienda/RichiestaInRevisione", new PraticheAziendaMail
                {
                    Nominativo = _nome,
                    Descrizione = motivazioni.Motivazione,
                    Note = praticheRegionaliImpreseStatoPratica.Note
                });

                var _t1 = "Richiesta annullata";
                var _t2 = "Eblig - Avviso Richiesta annullata";
                var _t3 = "Richiesta " + _richiesta.TipoRichiesta.Descrizione + " e stata annullata";

                if (_statoEnum == EbligEnums.StatoPratica.Revisione)
                {
                    _t1 = "Richiesta in revisione";
                    _t2 = "Eblig - Avviso Richiesta in revisione";
                    _t3 = "Richiesta " + _richiesta.TipoRichiesta.Descrizione + " e in revisione, verificare la corettezza dei dati inseriti";
                }

                AvvisoMail(_email, _nome, _t2, _body);

                UpdateListRicerca(_richiesta.PraticheRegionaliImpreseId);

                if (User.IsInRole(IdentityHelper.Roles.Admin.ToString()))
                {
                    AvvisaUtente(_richiesta.PraticheRegionaliImpreseId, "<strong>Informazione:</strong><br/>" +_t3);
                }

                return JsonResultTrue(_t1);
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [AuthorizeAdmin]
        public ActionResult VisualizzaBudget()
        {
            UnitOfWork _unitOfWork = new UnitOfWork();
            var _tipoRicieste = _unitOfWork.TipoRichiestaRepository.Get();

            var model = (from t in _tipoRicieste
                     select new VisualizzaBudgetViewModel
                     {
                         TipoRichiesta = t,
                         ImportoRichiestoRevisione = t.PraticheRegionaliImprese
                         .Where(x => x.StatoPraticaId == (int)EbligEnums.StatoPratica.Revisione)
                         .Sum(x => x.ImportoContributoNetto),
                         ImportoRichiestoConfermato = t.PraticheRegionaliImprese
                         .Where(x => x.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata)
                         .Sum(x => x.ImportoContributoNetto),
                         ImportoRichiestoBozza = t.PraticheRegionaliImprese
                         .Where(x => x.StatoPraticaId == (int)EbligEnums.StatoPratica.Bozza)
                         .Sum(x => x.ImportoContributoNetto),
                         ImportoRichiesto = t.PraticheRegionaliImprese
                         .Where(x => x.StatoPraticaId == (int)EbligEnums.StatoPratica.Inviata
                         || x.StatoPraticaId == (int)EbligEnums.StatoPratica.InviataRevisionata
                         ).Sum(x => x.ImportoContributoNetto)
                     });

            return AjaxView("VisualizzaBudget", model);
        }

        #region richiesta calcoli e controlli

        ActionResult GetImportoCalcolatiResult(int tipoRichiestaId, decimal? importoContributo)
        {
            var _tiporichiesta = GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == tipoRichiestaId);

            var _aliquoteIRPEF = _tiporichiesta.AliquoteIRPEF.GetValueOrDefault();

            var _importoIREF = importoContributo / 100 * _aliquoteIRPEF;

            var _importoContributoNetto = importoContributo - _importoIREF;

            var _importiCalcolati = new PraticheAzienda_ImportoCalcolatiModel
            {
                AliquoteIRPEF = _aliquoteIRPEF,
                ImportoIRPEF = _importoIREF,
                ImportoContributo = importoContributo,
                ImportoContributoNetto = _importoContributoNetto
            };

            var _htmlCalcoli = PartialView("~/Areas/Backend/Views/PraticheAzienda/ImportoCalcolati.cshtml").RenderViewToString(_importiCalcolati);

            return Json(new { html = _htmlCalcoli, importiCalcolati = _importiCalcolati }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetImportoErogatoIncentiviImpreseCovid19(int giorni, int tipoRichiestaId)
        {
            var _importoLordo = PraticheAziendaUtility.GetImportoErogatoIncentiviCovid19Imprese(giorni);
            return GetImportoCalcolatiResult(tipoRichiestaId, _importoLordo);
        }

        [HttpPost]
        public ActionResult GetImportoTotaleRimborsatoSicurezzaLavoroImprese(string importoAccettato, int tipoRichiestaId)
        {
            decimal.TryParse(importoAccettato, out decimal _importoAccettato);
            var _importoLordo = PraticheAziendaUtility.GetImportoTotaleRimborsatoSicurezzaLavoroImprese(_importoAccettato);
            return GetImportoCalcolatiResult(tipoRichiestaId, _importoLordo);
        }

        public ActionResult GetImportoTotaleRimborsatoQualitaInnovazioneImprese(string importoAccettato, int tipoRichiestaId)
        {
            decimal.TryParse(importoAccettato, out decimal _importoAccettato);
            var _importoRimborsato = PraticheAziendaUtility.GetImportoTotaleRimborsatoQualitaInnovazioneImprese(_importoAccettato);
            return GetImportoCalcolatiResult(tipoRichiestaId, _importoRimborsato);
        }

        public ActionResult GetImportoEventiEccezionaliCalamitaNaturaliImprese(string danniAttrezzatura, string danniScorte, int tipoRichiestaId)
        {
            decimal.TryParse(danniAttrezzatura, out decimal _danniAttrezzatura);
            decimal.TryParse(danniScorte, out decimal _danniScorte);
            var _importoRimborsato = PraticheAziendaUtility.GetImportoEventiEccezionaliCalamitaNaturaliImprese(_danniAttrezzatura, _danniScorte);
            return GetImportoCalcolatiResult(tipoRichiestaId, _importoRimborsato);
        }

        public ActionResult GetImportoIncrementoMantenimentoOccupazionImprese(int ore, int tipoRichiestaId)
        {
            var _importoRimborsato = PraticheAziendaUtility.GetImportoIncrementoMantenimentoOccupazionImprese(ore);
            return GetImportoCalcolatiResult(tipoRichiestaId, _importoRimborsato);
        }

        public ActionResult GetImportoIncrementoMantenimentoOccupazionImpreseTotale(int importoTotale, int tipoRichiestaId)
        {
            return GetImportoCalcolatiResult(tipoRichiestaId, importoTotale);
        }

        public ActionResult VerificaTipoRichiestaUnivocoCodiceFiscale(int aziendaId, int tipoRichiestaId, string codiceFiscale, int richiestaId, string nomeCampo, bool unica)
        {
            var _exists = PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(aziendaId, tipoRichiestaId, codiceFiscale, richiestaId, nomeCampo, unica);
            return Json(new { isValid = !_exists, message = _exists ? "Per il Codice Fiscale e già stata presentata una richiesta" : "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RimuoviEventoMalattia(int index, List<EventiMalattia> childClass)
        {
            try
            {
                childClass.RemoveAt(index);

                PraticheAzienda_Dipendente_CarenzaMalattia_Eventi _model = new PraticheAzienda_Dipendente_CarenzaMalattia_Eventi
                {
                    ReadOnly = false,
                    ChildClass = childClass
                };

                var _html = PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/CarenzaMalattiaDipendente_Eventi.cshtml").RenderViewToString(_model);

                return Json(new { isValid = true, totale = childClass.Count(), list = _html });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult AggiungiEventoMalattia(EventiMalattia model, List<EventiMalattia> childClass)
        {
            try
            {
                var _errors = IsValidModel(new object[] { model });

                if (_errors.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_errors));
                }

                if (childClass == null)
                {
                    childClass = new List<EventiMalattia>();
                }

                //check date
                foreach (var item in childClass)
                {
                    var _datafine = item.Data.GetValueOrDefault().AddDays(item.Giorni.GetValueOrDefault());

                    if (model.Data >= item.Data && model.Data <= _datafine)
                    {
                        throw new Exception("Data malattià già selezionata");
                    }

                    var _data = model.Data.GetValueOrDefault().AddDays(model.Giorni.GetValueOrDefault());

                    if (_data >= item.Data && _data <= _datafine)
                    {
                        throw new Exception("Data malattià già selezionata");
                    }

                }

                childClass.Add(model);

                PraticheAzienda_Dipendente_CarenzaMalattia_Eventi _model = new PraticheAzienda_Dipendente_CarenzaMalattia_Eventi
                {
                    ReadOnly = false,
                    ChildClass = childClass
                };

                var _html = PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/CarenzaMalattiaDipendente_Eventi.cshtml").RenderViewToString(_model);

                return Json(new { isValid = true, totale = childClass.Count(), list = _html });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult AggiungiParentela(PraticheAzienda_Dipendente_Parentela model, List<PraticheAzienda_Dipendente_Parentela> childClass)//DateTime data, int giorni)
        {
            try
            {
                var _errors = IsValidModel(new object[] { model });

                if (_errors.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_errors));
                }

                if (childClass == null)
                {
                    childClass = new List<PraticheAzienda_Dipendente_Parentela>();
                }

                //check date
                //foreach (var item in childClass)
                //{
                //    var _datafine = item.Data.GetValueOrDefault().AddDays(item.Giorni.GetValueOrDefault());

                //    if (data >= item.Data && data <= _datafine)
                //    {
                //        throw new Exception("Data malattià già selezionata");
                //    }

                //    var _data = data.AddDays(giorni);

                //    if (_data >= item.Data && _data <= _datafine)
                //    {
                //        throw new Exception("Data malattià già selezionata");
                //    }

                //}

                childClass.Add(model);

                PraticheAzienda_Dipendente_Parenti _model = new PraticheAzienda_Dipendente_Parenti
                {
                    ReadOnly = false,
                    ChildClass = childClass
                };

                var _html = PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/Dipendente_Parentela.cshtml").RenderViewToString(_model);

                return Json(new { isValid = true, totale = childClass.Count(), list = _html });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult RimuoviParentela(int index, List<PraticheAzienda_Dipendente_Parentela> childClass)
        {
            try
            {
                childClass.RemoveAt(index);

                PraticheAzienda_Dipendente_Parenti _model = new PraticheAzienda_Dipendente_Parenti
                {
                    ReadOnly = false,
                    ChildClass = childClass
                };

                var _html = PartialView("~/Areas/Backend/Views/PraticheAzienda/TipoRichiesta/Dipendente_Parentela.cshtml").RenderViewToString(_model);

                return Json(new { isValid = true, totale = childClass.Count(), list = _html });
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region richiesta helper

        private ICollection<PraticheRegionaliImpreseDatiPratica> CreateDatiPratica<T>(T o, ref decimal? _importoDaRimborsare, ref bool? hashImportoColumn)
        {
            try
            {
                List<PraticheRegionaliImpreseDatiPratica> _list = new List<PraticheRegionaliImpreseDatiPratica>();

                if (o == null)
                {
                    return _list;
                }

                foreach (var item in o.GetType().GetProperties())
                {
                    try
                    {
                        var _excludet = false;
                        foreach (var _attributes in item.GetCustomAttributes(true))
                        {
                            if (_attributes is NotMappedAttribute)
                            {
                                _excludet = true;
                                break;
                            }
                        }

                        foreach (var _attributes in item.GetCustomAttributes(true))
                        {
                            if (_attributes is PraticheAzienda_ImportoContributoAttribute)
                            {
                                hashImportoColumn = true;
                                if (o != null && item.GetValue(o) != null)
                                {
                                    decimal.TryParse(item.GetValue(o).ToString(), out decimal a);
                                    _importoDaRimborsare += a;
                                }
                            }
                        }

                        if (_excludet)
                        {
                            continue;
                        }

                        _list.Add(new PraticheRegionaliImpreseDatiPratica
                        {
                            Nome = item.Name,
                            Valore = o != null && item.GetValue(o) != null ? item.GetValue(o).ToString() : null
                        });
                    }
                    catch
                    {
                    }
                }

                return _list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ICollection<PraticheRegionaliImpreseAllegati> CreateAllegati(PraticheAziendaAllegati upload, int praticheRegionaliImpreseId)
        {
            try
            {
                List<PraticheRegionaliImpreseAllegati> _list = new List<PraticheRegionaliImpreseAllegati>();

                if (upload == null || upload?.File == null || upload?.File?.Count() == 0)
                {
                    return _list;
                }

                var cartellaServer = GetUploadFolder(PathPraticheAzienda, praticheRegionaliImpreseId);

                foreach (var item in upload.File)
                {
                    if (item.PraticheRegionaliImpreseAllegatiId.GetValueOrDefault() != 0)
                    {
                        continue;
                    }

                    var filename = Savefile(cartellaServer, item.Base64);

                    _list.Add(new PraticheRegionaliImpreseAllegati
                    {
                        Filename = filename,
                        FilenameOriginale = item.NomeFile,
                        TipoRichiestaAllegatiId = item.TipoRichiestaAllegatiId,
                        PraticheRegionaliImpreseId = praticheRegionaliImpreseId,
                    });
                }

                return _list;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private object CreateModelDatiRichiestaChildClass(TipoRichiesta tiporichiesta, IEnumerable<PraticheRegionaliImpreseDatiPratica> datiRichiesta, int? childClassRowCount = 0)
        {
            if (childClassRowCount.GetValueOrDefault() == 0)
            {
                return null;
            }

            IList _listDatiPraticaEventi = CreateInstanceList(tiporichiesta.ChildClass);

            for (int i = 0; i < childClassRowCount; i++)
            {
                var model = CreateInstance(tiporichiesta.ChildClass);

                foreach (var item in model.GetType().GetProperties())
                {
                    var dati = datiRichiesta.FirstOrDefault(x => x.Nome == "ChildClass" + "[" + i + "]." + item.Name);

                    Reflection.SetValue(model, dati?.Nome.Replace("ChildClass" + "[" + i + "].", ""), dati?.Valore);
                }

                _listDatiPraticaEventi.Add(model);
            }

            return _listDatiPraticaEventi;
        }

        private object CreateModelDatiRichiesta(TipoRichiesta tiporichiesta, IEnumerable<PraticheRegionaliImpreseDatiPratica> datiRichiesta)
        {
            var model = CreateInstance(tiporichiesta.Classe);

            foreach (var item in model.GetType().GetProperties())
            {

                //if (typeof(T).Name != item.DeclaringType.Name)
                //    continue;


                var dati = datiRichiesta.FirstOrDefault(x => x.Nome == item?.Name);

                Reflection.SetValue(model, dati?.Nome, dati?.Valore);
            }

            return model;
        }

        private IList CreateInstanceList(string classe)
        {
            if (string.IsNullOrWhiteSpace(classe))
            {
                return default;
            }

            var model = CreateInstance(classe);

            // Create a list of the required type and cast to IList
            Type genericListType = typeof(List<>);
            Type concreteListType = genericListType.MakeGenericType(model.GetType());
            IList _list = Activator.CreateInstance(concreteListType) as IList;

            return _list;
        }

        private object CreateInstance(string classe)
        {
            //var x =  typeof(PraticheAzienda_IncrementoMantenimentoOccupazione.Richiedente).AssemblyQualifiedName;
            if (string.IsNullOrWhiteSpace(classe))
            {
                return default;
            }

            Type t = Type.GetType(classe);
            return Activator.CreateInstance(t);
        }

        private void SetProvicia(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.ProvinceRepository.Get(xx => xx.ProvinciaId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetComune(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.ComuniRepository.Get(xx => xx.ComuneId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetRegione(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.RegioniRepository.Get(xx => xx.RegioneId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetLocalita(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.LocalitaRepository.Get(xx => xx.LocalitaId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);
                }
            }
            catch
            {

            }
        }

        private void SetProperty(object model, string prop, object value)
        {
            try
            {
                model.GetType().GetProperty(prop)?.SetValue(model, value);
            }
            catch
            {

            }
        }

        private void DeleteFiles(List<string> filesToDelete, int praticheRegionaliImpreseId)
        {
            var cartellaServer = GetUploadFolder(PathPraticheAzienda, praticheRegionaliImpreseId);

            foreach (var filename in filesToDelete)
            {
                try
                {
                    System.IO.File.Delete(System.IO.Path.Combine(cartellaServer, filename));
                }
                catch
                {
                }
            }
        }

        private List<string> IsValidModel(object[] value)
        {
            try
            {
                var results = new List<ValidationResult>();

                foreach (var item in value)
                {
                    var context = new ValidationContext(item, null, null);

                    Validator.TryValidateObject(item, context, results, true);
                }

                return results?.Select(x => x.ErrorMessage)?.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<TipoRichiestaAllegati> GetTipoRichiestaAllegati(int id)
        {
            return unitOfWork.TipoRichiestaAllegatiRepository.Get(xx => xx.TipoRichiestaId == id).ToList();
        }

        private List<StatoPratica> GetStatoPratica()
        {
            return unitOfWork.StatoPraticaRepository.Get().ToList();
        }

        public List<TipoRichiesta> GetTipoRichieste()
        {
            using (var _unitofwork = new UnitOfWork())
            {
                var result = new List<TipoRichiesta>();

                if (IsUserAzienda || IsUserConsulenteCs)
                {
                    result = _unitofwork.TipoRichiestaRepository.Get(x => x.IsTipoRichiestaDipendente != true).ToList();
                }
                else if (IsUserDipendente || IsUserSportello)
                {
                    result = _unitofwork.TipoRichiestaRepository.Get(x => x.IsTipoRichiestaDipendente == true).ToList();
                }
                else
                {
                    result = _unitofwork.TipoRichiestaRepository.Get().ToList();
                }

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        item.Descrizione = item.Descrizione + " (" + item.Anno + ")";
                    }
                }

                return result.OrderBy(x => x.Descrizione).ToList();
            }
        }

        public void VerificaCoperturaAzienda(int aziendaId)
        {
            try
            {
                using (var _unitofwork = new UnitOfWork())
                {

                    var _az = _unitofwork.CoperturaRepository.Get(x => x.AziendaId == aziendaId).FirstOrDefault();
                    if (_az != null && _az.Coperto == false)
                    {
                        throw new Exception("L'azienda non risulta in regola con i contributi");
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CheckUserAbilitatoRichiesta(PraticheRegionaliImprese richiesta)
        {
            if (richiesta != null)
            {
                //verifica che utente e il proprietario della richiesta
                if (IsUserConsulenteCs)
                {
                    if (richiesta.ConsulenteCSId != GetConsulenteCsId || richiesta.DipendenteId != null)
                    {
                        throw new Exception("Consulente/Cs non abilitato visualizza la richiesta");
                    }
                }

                if (IsUserAzienda)
                {
                    if (richiesta.AziendaId != GetAziendaId || richiesta.DipendenteId != null)
                    {
                        throw new Exception("Azienda non abilitato visualizza la richiesta");
                    }
                }

                if (IsUserDipendente)
                {
                    if (richiesta.DipendenteId != GetDipendenteId)
                    {
                        throw new Exception("Dipendente non abilitato visualizza la richiesta");
                    }
                }
            }
        }

        private string GetNominativoFromRichiesta(PraticheRegionaliImprese richiesta)
        {
            var _nome = "";

            if (richiesta.DipendenteId.HasValue)
            {
                _nome = richiesta.Dipendente?.Nome + " " + richiesta.Dipendente?.Cognome;
            }
            else if (richiesta.ConsulenteCSId.HasValue)
            {
                _nome = richiesta.ConsulenteCS?.Nome + " " + richiesta.ConsulenteCS?.Cognome;
            }
            else if (richiesta.SportelloId.HasValue)
            {
                _nome = richiesta.Sportello?.Nome + " " + richiesta.Sportello?.Cognome;
            }
            else
            {
                _nome = richiesta.Azienda.RagioneSociale;
            }
            return _nome;
        }

        private string GetEmailAddressFromRichiesta(PraticheRegionaliImprese richiesta)
        {
            var _email = "";

            if (richiesta.DipendenteId.HasValue)
            {
                _email = richiesta.Dipendente?.Email;
            }
            else if (richiesta.SportelloId.HasValue)
            {
                _email = richiesta.Sportello?.Email;
            }
            else if (richiesta.ConsulenteCSId.HasValue)
            {
                _email = richiesta.ConsulenteCS?.Email;
            }
            else
            {
                _email = richiesta.Azienda?.Email;
            }

            return _email;
        }

        private void AvvisoMail(string email, string nome, string subject, string body)
        {
            try
            {
                Task.Run(() =>
                {
                    //Invia una mail che la richiesta e in revisione
                    SendMailAsync(new WebUI.Models.SimpleMailMessage
                    {
                        ToEmail = email,
                        ToName = nome,
                        Subject = subject,
                        Body = body
                    });
                });
            }
            catch
            {

            }
        }

        private void UpdateListRicerca(int richiestaId)
        {
            try
            {
                var richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();

                List<string> _usernames = new List<string>
                {
                    richiesta.UserInserimento
                };

                //update list ricerca
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();

                if (richiesta.DipendenteId == null && richiesta.Azienda != null)
                {
                    _usernames.Add(richiesta.Azienda?.MatricolaInps);
                }

                if (richiesta.DipendenteId == null && richiesta.ConsulenteCS != null)
                {
                    _usernames.Add(richiesta.ConsulenteCS?.CodiceFiscalePIva);
                }

                if (richiesta.DipendenteId != null)
                {
                    _usernames.Add(richiesta.Dipendente?.CodiceFiscale);
                }

                foreach (var item in _usernames.Distinct())
                {
                    context.Clients.All.onUpdateListRicerca(item);
                }
            }
            catch
            {
            }
        }

        private void AvvisaAdmin(string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onAvvisaAdmin(message);
        }

        private void AvvisaUtente(int richiestaId, string message)
        {
            try
            {
                var richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                List<string> _usernames = new List<string>();
                _usernames.Add(richiesta.UserInserimento?.ToUpper());

                //update list ricerca
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();

                if (richiesta.DipendenteId == null && richiesta.Azienda != null)
                {
                    _usernames.Add(richiesta.Azienda?.MatricolaInps?.ToUpper());
                }

                if (richiesta.DipendenteId == null && richiesta.ConsulenteCS != null)
                {
                    _usernames.Add(richiesta.ConsulenteCS?.CodiceFiscalePIva?.ToUpper());
                }

                if (richiesta.DipendenteId != null)
                {
                    _usernames.Add(richiesta.Dipendente?.CodiceFiscale?.ToUpper());
                }

                if (richiesta.Sportello != null)
                {
                    _usernames.Add(richiesta.Sportello?.CodiceFiscalePIva?.ToUpper());
                }

                foreach (var item in _usernames.Distinct())
                {
                    context.Clients.All.onAvvisoUtente(item, message);
                }
            }
            catch
            {
            }
        }

        public void ConfermaRichiestaMail(PraticheRegionaliImprese richiesta)
        {
            try
            {
                //invia mail
                var _email = GetEmailAddressFromRichiesta(richiesta);
                var _nome = GetNominativoFromRichiesta(richiesta);

                var _body = RenderTemplate("PraticheAzienda/RichiestaConfermata", new PraticheAziendaMail
                {
                    Nominativo = _nome,
                    Descrizione = richiesta.ProtocolloId
                });

                AvvisoMail(_email, _nome, "Eblig - Avviso Richiesta confermata", _body);
            }
            catch
            {
            }
        }

        #endregion

        #region helper

        internal JsonResult JsonResultTrue(int? richiestaId, string message)
        {

            return Json(new
            {
                richiestaId,
                isValid = true,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }

        internal JsonResult JsonResultFalse(int? richiestaId, string message)
        {
            return Json(new
            {
                richiestaId,
                isValid = false,
                message,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
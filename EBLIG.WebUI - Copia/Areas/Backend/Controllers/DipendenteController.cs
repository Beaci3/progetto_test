using DocumentFormat.OpenXml.EMMA;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using Sediin.MVC.HtmlHelpers;
using System;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class DipendenteController : BaseController
    {
        public string PathCartaIdentita { get => "Documenti\\Sportello\\{0}\\CartaIdentita"; private set { } }
        public string PathDelegheDipendente { get => "Documenti\\Sportello\\{0}\\Delega\\Dipendente"; private set { } }
        public string PathCartaIdentitaDipendente { get => "Documenti\\Dipendente\\{0}\\CartaIdentita"; private set { } }
        public string PathAltroDipendente { get => "Documenti\\Dipendente\\{0}\\Altro"; private set { } }

        #region ricerca

        [AuthorizeSportello]
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [AuthorizeSportello]
        public ActionResult Ricerca(DipendenteRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.DipendenteRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);
            // var _query = await unitOfWork.DipendenteRepository.GetAsync(RicercaFilter(model));

            var _result = GeModelWithPaging<DipendenteRicercaViewModel, Dipendente>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [AuthorizeSportello]
        public ActionResult RicercaExcel(DipendenteRicercaModel model)
        {
            var _query = from a in unitOfWork.DipendenteRepository.Get(RicercaFilter(model)).OrderBy(r => r.Cognome)
                         select new
                         {
                             a.Cognome,
                             a.Nome,
                             a.CodiceFiscale,
                             a.Datanascita,
                             a.Email,
                             a.Cellulare,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Dipendenti");
        }

        private Expression<Func<Dipendente, bool>> RicercaFilter(DipendenteRicercaModel model)
        {
            int? sportelloId = null;

            if (IsUserSportello)
            {
                sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            return x =>
            (sportelloId != null ? (x.SportelloId == sportelloId) : true)
            && (model.DipendenteRicercaModel_Cognome != null ? x.Cognome.Contains(model.DipendenteRicercaModel_Cognome) : true
            && model.DipendenteRicercaModel_CodiceFiscale != null ? x.CodiceFiscale == model.DipendenteRicercaModel_CodiceFiscale : true
            && model.DipendenteRicercaModel_ComuneId != null ? x.ComuneId == model.DipendenteRicercaModel_ComuneId : true);
        }

        private SqlLam<Dipendente> RicercaFilter2(DipendenteRicercaModel model)
        {
            int? sportelloId = null;

            if (IsUserSportello)
            {
                sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            var f = new SqlLam<Dipendente>();

            if (sportelloId.HasValue)
            {
                f.And(x => x.SportelloId == sportelloId);
            }

            if (!string.IsNullOrWhiteSpace(model.DipendenteRicercaModel_Cognome))
            {
                f.And(xd => xd.Cognome.Contains(model.DipendenteRicercaModel_Cognome));
            }

            if (!string.IsNullOrWhiteSpace(model.DipendenteRicercaModel_CodiceFiscale))
            {
                f.And(xd => xd.CodiceFiscale.Contains(model.DipendenteRicercaModel_CodiceFiscale));
            }

            if (model.DipendenteRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.DipendenteRicercaModel_ComuneId);
            }
            return f;
        }

        #endregion

        #region associa dipendente a sportello sindacale

        [AuthorizeSportello]
        public ActionResult AssociaDipendenteRicerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [AuthorizeSportello]
        public async Task<ActionResult> AssociaDipendenteRicerca(DipendenteAssociaRicercaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                int? sportelloId = GetSportelloId;

                Expression<Func<Dipendente, bool>> _filter = x =>
                (sportelloId != null ? (x.SportelloId != sportelloId) : true)
                && ((x.DipendenteId == model.DipendenteAssociaRicercaModel_DipendenteId));

                var _result = await unitOfWork.DipendenteRepository.GetAsync(_filter);
                if (_result.Count() == 0)
                {
                    return JsonResultFalse("Dipendente non trovata.");
                }
                DipendenteAssociaRicercaViewModel dipAssociaRicercaViewModel = new DipendenteAssociaRicercaViewModel();
                dipAssociaRicercaViewModel.Dipendente = _result;
                return AjaxView("AssociaDipendenteRicercaList", dipAssociaRicercaViewModel);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AuthorizeSportello]
        public ActionResult AssociaDipendente(DipendenteAssociaRicercaViewModel model)
        {
            try
            {
                AssociaSportelloDipendente(model.DipendenteId, GetSportelloId.Value, model.DelegaDipendente, model.DocumentoIdentita);
                return JsonResultTrue("Dipendente associato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        void AssociaSportelloDipendente(int dipendenteId, int sportelloId, string delegaDipendente, string documentoIdentita)
        {
            try
            {
                var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == dipendenteId).FirstOrDefault();

                if (_dipendente == null)
                {
                    throw new Exception("Dipendente non trovato");
                }

                var _delegaattiva = unitOfWork.DelegheSportelloDipendenteRepository.Get(xx => xx.DelegaAttiva == true && xx.DipendenteId == dipendenteId);

                if (_delegaattiva != null)
                {
                    foreach (var item in _delegaattiva)
                    {
                        item.DataDelegaDisdetta = DateTime.Now;
                        item.DelegaAttiva = false;
                        unitOfWork.DelegheSportelloDipendenteRepository.Update(item);
                    }
                }

                DelegheSportelloDipendente _delega = new DelegheSportelloDipendente
                {
                    DelegaAttiva = true,
                    DipendenteId = dipendenteId,
                    SportelloId = sportelloId,
                    DataInserimento = DateTime.Now,
                    DelegaDipendente = Savefile(GetUploadFolder(PathDelegheDipendente, GetSportelloId.Value), delegaDipendente),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, GetSportelloId.Value), documentoIdentita),
                };
                unitOfWork.DelegheSportelloDipendenteRepository.Insert(_delega);

                //aggiorna sportelloId tabella dipendenti
                _dipendente.SportelloId = sportelloId;


                unitOfWork.Save(false);

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        [Authorize(Roles = "Admin, Dipendente, Sportello")]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                Expression<Func<Dipendente, bool>> _filter = x => x.DipendenteId == id;

                //utente visualizza solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    _filter = x => x.CodiceFiscale == User.Identity.Name;

                }

                var _outModel = new DipendenteViewModel();

                var _result = unitOfWork.DipendenteRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    _outModel = Reflection.CreateModel<DipendenteViewModel>(_result);
                    _outModel.Aziende = _result.Aziende;
                    _outModel.Sportello = _result.Sportello;
                    _outModel.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                    {
                        _outModel = new DipendenteViewModel
                        {
                            CodiceFiscale = _user.UserName,
                            Cognome = _user.Cognome,
                            Nome = _user.Nome,
                            Email = _user.Email,
                            InformazioniPersonaliCompilati = false
                        };
                    }
                }

                return AjaxView("Anagrafica", _outModel);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "DipendenteController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Dipendente, Sportello")]
        public async Task<ActionResult> Anagrafica(DipendenteViewModel model)
        {
            try
            {
                //utente aggiorna solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    model.CodiceFiscale = User.Identity.Name;

                    Expression<Func<Dipendente, bool>> _filter = x => x.CodiceFiscale == User.Identity.Name;

                    var _result = unitOfWork.DipendenteRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.DipendenteId = _result.DipendenteId;
                        model.CodiceFiscale = _result.CodiceFiscale;
                    }
                }

                if (model.DipendenteId != 0)
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaDipendente");
                }

                if (!User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaDipendente");
                }

                //check dipendente exists con codice fiscale
                var _DipendenteExists = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId != model.DipendenteId
                && x.CodiceFiscale.ToLower() == model.CodiceFiscale.ToLower());
                if (_DipendenteExists?.Count() > 0)
                {
                    throw new Exception("Codice Fiscale già registrata");
                }

                if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()) && model.DipendenteId != 0)
                {
                    var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == model.DipendenteId);

                    if (_dipendente?.Count() > 0 && _dipendente.FirstOrDefault()?.SportelloId != GetSportelloId)
                    {
                        throw new Exception("Dipendente non e associata alla sua Utenza");
                    }

                    model.SportelloId = GetSportelloId;
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to Dipendente model
                var _resultModel = Reflection.CreateModel<Dipendente>(model);
                _resultModel.CodiceFiscale = _resultModel.CodiceFiscale.ToUpper();
                unitOfWork.DipendenteRepository.InsertOrUpdate(_resultModel);
                unitOfWork.Save(false);

                //inserisci DelegheSportelloDipendente
                if (model.DipendenteId == 0 && User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                {
                    AssociaSportelloDipendente(_resultModel.DipendenteId, GetSportelloId.Value, model.DelegaDipendente, model.DocumentoIdentita);
                }

                //update flag InformazioniPersonaliCompilati
                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                    if (!_user.InformazioniPersonaliCompilati)
                    {
                        _user.InformazioniPersonaliCompilati = true;
                        await UserManager.UpdateAsync(_user);
                    }
                }

                return Json(new
                {
                    isValid = true,
                    DipendenteId = _resultModel.DipendenteId,
                    message = "Anagrafica " + (model.DipendenteId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public JsonResult ListaDipendentiSportello(string phrase)
        {
            Expression<Func<Dipendente, bool>> _filter = x =>
            (x.SportelloId == GetSportelloId)
            && ((phrase != null ? (x.Nome + " " + x.Cognome).Contains(phrase) : true)
            || (phrase != null ? x.CodiceFiscale.Contains(phrase) : true));

            return GetListaDipendenti(_filter);
        }

        public JsonResult ListaDipendentiAll(string phrase)
        {
            Expression<Func<Dipendente, bool>> _filter = x =>
            ((phrase != null ? (x.Nome + " " + x.Cognome).Contains(phrase) : true)
            || (phrase != null ? x.CodiceFiscale.Contains(phrase) : true));

            return GetListaDipendenti(_filter);
        }

        private JsonResult GetListaDipendenti(Expression<Func<Dipendente, bool>> filter)
        {
            var _result = unitOfWork.DipendenteRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Cognome == null || p.Cognome == "")
                       .ThenBy(p => p.Nome == null || p.Nome == "")
                       .ThenBy(p => p.CodiceFiscale)
                       .Select(x => new { x.DipendenteId, x.CodiceFiscale, Nominativo = x.Nome + " " + x.Cognome + " - " + x.CodiceFiscale }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        #region aziende associate al dipendente

        [AuthorizeDipendenteSportello]
        public ActionResult AziendeAssociateRicerca()
        {
            return AjaxView();
        }

        [AuthorizeDipendenteSportello]
        [HttpPost]
        public ActionResult AziendeAssociateRicerca(DipendenteAssociaAziendaRicercaModel model = null)
        {
            if (model == null)
            {
                model = new DipendenteAssociaAziendaRicercaModel();
            }

            var _aziende = unitOfWork.DipendenteAziendaRepository.Get(x =>
            (x.DipendenteId == (IsUserDipendente ? (int)GetDipendenteId.Value : model.DipendenteAssociaRicercaModel_DipendenteId))
            && (IsUserSportello ? x.Dipendente.SportelloId == (int)GetSportelloId.Value : true)
            );

            return AjaxView("AziendeAssociateRicercaList", _aziende);
        }

        #endregion

        #region associa azienda

        [AuthorizeDipendenteSportello]
        public ActionResult AziendaAssociaRicerca()
        {
            if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
            {
                DipendenteTempoPieno(GetDipendenteId.Value);
            }

            return AjaxView();

        }

        [HttpPost]
        [AuthorizeDipendenteSportello]
        public async Task<ActionResult> AziendaAssociaRicerca(AziendaAssociaRicercaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    DipendenteTempoPieno(GetDipendenteId.Value);
                }

                var _aziendeAssociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == (int)GetDipendenteId.Value
                && x.DataCessazione == null);

                var _aziendeAssociateId = _aziendeAssociate?.Select(x => x.AziendaId);

                Expression<Func<Azienda, bool>> _filter = x =>
                (_aziendeAssociateId.Count() > 0 ? !_aziendeAssociateId.Contains(x.AziendaId) : true)
                && ((x.AziendaId == model.AziendaAssociaRicercaModel_AziendaId));

                var _result = await unitOfWork.AziendaRepository.GetAsync(_filter);

                if (_result.Count() == 0)
                {
                    return JsonResultFalse("Azienda non censita in anagrafica. Contattare gli uffici di EBLIG.");
                }

                DipendenteAziendaAssociaViewModel aziendaAssociaRicercaViewModel = new DipendenteAziendaAssociaViewModel
                {
                    Aziende = _result,
                    TipoContratto = await unitOfWork.TipoContrattoRepository.GetAsync(),
                    //se ha aziende associate, rimuovere le voci con flag "tempo pieno" da "TempoLavoro"
                    TempoLavoro = await unitOfWork.TempoLavoroRepository.GetAsync(x => _aziendeAssociateId.Count() > 0 ? (bool)x.TempoPieno != true : true),
                    TipoImpiego = await unitOfWork.TipoImpiegoRepository.GetAsync()
                };

                return AjaxView("AziendaAssociaRicercaList", aziendaAssociaRicercaViewModel);

            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [HttpPost]
        [AuthorizeDipendenteSportello]
        [ValidateAntiForgeryToken]
        public ActionResult AssociaAzienda(DipendenteAziendaAssociaViewModel model)
        {
            try
            {
                int dipendenteId = model.DipendenteAziendaAssociaViewModel_DipendenteId.GetValueOrDefault();

                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    ModelState.Remove("DipendenteAziendaAssociaViewModel_DipendenteId");
                    dipendenteId = GetDipendenteId.Value;
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                DipendenteTempoPieno(dipendenteId);

                DipendenteAzienda dipendenteAzienda = new DipendenteAzienda
                {
                    AziendaId = model.AziendaId,
                    CCNLCNEL = model.CCNLCNEL,
                    DataAssunzione = model.DataAssunzione.GetValueOrDefault(),
                    DipendenteId = dipendenteId,
                    TempoLavoroId = model.TempoLavoroId.GetValueOrDefault(),
                    TipoContrattoId = model.TipoContrattoId.GetValueOrDefault(),
                    TipoImpiegoId = model.TipoImpiegoId.GetValueOrDefault(),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentitaDipendente, dipendenteId), model.DocumentoIdentita),
                    //DocumentoAltro = Savefile(GetUploadFolder(PathAltroDipendente, dipendenteId), model.AltroDocumento),
                };

                unitOfWork.DipendenteAziendaRepository.Insert(dipendenteAzienda);
                unitOfWork.Save(false);

                return JsonResultTrue("Azienda associata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [HttpPost]
        [AuthorizeDipendenteSportello]
        [ValidateAntiForgeryToken]
        public ActionResult CessazioneContratto(DipendenteAziendaCessazioneContrattoModel model)
        {
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return AjaxView("AziendeAssociateRicerca");
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                var _dipaz = unitOfWork.DipendenteAziendaRepository.Get(x =>
                x.DipendenteAziendaId == model.DipendenteAziendaId
                && (IsUserSportello ? x.Dipendente.SportelloId == (int)GetSportelloId.Value : true)
                && (IsUserDipendente ? x.DipendenteId == (int)GetDipendenteId.Value : true)).FirstOrDefault();

                if (_dipaz == null)
                {
                    throw new Exception("Richiesta non valida");
                }

                _dipaz.DataAssunzione = model.DataAssunzione;
                _dipaz.DataCessazione = model.DataCessione;
                unitOfWork.DipendenteAziendaRepository.Update(_dipaz);
                unitOfWork.Save(false);

                return JsonResultTrue("Dati aggiornati");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [AuthorizeDipendenteSportello]
        public ActionResult CessazioneContratto(int dipendenteAziendaId)
        {
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return AjaxView("AziendeAssociateRicerca");
                }

                var _dipaz = unitOfWork.DipendenteAziendaRepository.Get(x =>
                x.DipendenteAziendaId == dipendenteAziendaId
                && (IsUserSportello ? x.Dipendente.SportelloId == (int)GetSportelloId.Value : true)
                && (IsUserDipendente ? x.DipendenteId == (int)GetDipendenteId.Value : true)).FirstOrDefault();

                if (_dipaz == null)
                {
                    throw new Exception("Richiesta non valida");
                }

                if (_dipaz.DataCessazione.HasValue)
                {
                    throw new Exception("Richiesta non valida");
                }

                DipendenteAziendaCessazioneContrattoModel model = new DipendenteAziendaCessazioneContrattoModel();
                model.DipendenteAziendaId = _dipaz.DipendenteAziendaId;
                model.DataAssunzione = _dipaz.DataAssunzione;

                return AjaxView("CessazioneContratto", model);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Dipendente assunto a "Tempo Pieno"
        /// </summary>
        /// <exception cref="Exception"></exception>
        void DipendenteTempoPieno(int dipendenteId)
        {
            var _aziendeAssociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == dipendenteId && x.DataCessazione == null);

            //CONTROLLI:
            //Se ha inserito la tipologia “Tempo Pieno” non può associare più di un'azienda.
            //Se prova ad associare più di un'azienda e ha dichiarato di essere Tempo Pieno
            //possiamo far tornare il messaggio: “Impossibile associare più di un'azienda in
            //caso di assunzione a Tempo Pieno”
            if (_aziendeAssociate?.Where(d => d.DataCessazione == null && d.TempoLavoro.TempoPieno == true).Count() > 0)
            {
                throw new Exception("Impossibile associare più di un'azienda in caso di assunzione a Tempo Pieno");
            }
        }
        #endregion

        public ActionResult DownloadAllegato(int? delegaId, string allegato)
        {
            try
            {
                var _allegato = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.DelegheSportelloDipendenteId == delegaId).FirstOrDefault();

                var _uploadFolder = "";

                var _file = allegato == "DelegaDipendente" ? _allegato?.DelegaDipendente : _allegato?.DocumentoIdentita;

                if (allegato == "DelegaDipendente")
                {
                    _uploadFolder = GetUploadFolder(PathDelegheDipendente, _allegato.SportelloId);
                }
                else
                {
                    _uploadFolder = GetUploadFolder(PathCartaIdentita, _allegato.SportelloId);
                }

                if (_allegato == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _file)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = System.Web.MimeMapping.GetMimeMapping(_file);
                return File(Path.Combine(_uploadFolder, _file), mimeType, $"{allegato}.{Path.GetExtension(_file)}");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using EBLIG.DOM;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using MailKit.Search;
using Org.BouncyCastle.Bcpg;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class AziendaController : BaseController
    {
        public string PathCartaIdentita { get => "Documenti\\Consulente\\{0}\\CartaIdentita"; private set { } }
        public string PathDelegheAzienda { get => "Documenti\\Consulente\\{0}\\Delega\\Azienda"; private set { } }

        #region ricerca

        [AuthorizeConsulenteCs]
        public async Task<ActionResult> Ricerca()
        {
            AziendaRicercaModel model = new AziendaRicercaModel
            {
                Tipologie = await unitOfWork.TipologiaRepository.GetAsync(),
            };

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        [AuthorizeConsulenteCs]
        public ActionResult Ricerca(AziendaRicercaModel model, int? page)
        {
            int totalRows = 0;

            var _query = unitOfWork.AziendaRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<AziendaRicercaViewModel, Azienda>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [AuthorizeConsulenteCs]
        public ActionResult RicercaExcel(AziendaRicercaModel model)
        {
            var _query = from a in unitOfWork.AziendaRepository.Get(RicercaFilter(model)).OrderBy(r => r.RagioneSociale)
                         select new
                         {
                             a.RagioneSociale,
                             a.CognomeTitolare,
                             a.NomeTitolare,
                             a.CodiceFiscale,
                             a.PartitaIva,
                             a.MatricolaInps,
                             a.Tipologia?.Descrizione,
                             a.CSC,
                             a.Classificazione,
                             a.AttivitaEconomica,
                             a.CodiceIstat,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Aziende");
        }

        private SqlLam<Azienda> RicercaFilter2(AziendaRicercaModel model)
        {
            int? consulenteCs = null;

            if (IsUserConsulenteCs)
            {
                consulenteCs = GetConsulenteCsId.Value;
            }

            TrimAll(model);

            var f = new SqlLam<Azienda>();

            if (consulenteCs.HasValue)
            {
                f.And(x => x.ConsulenteCSId == consulenteCs);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_RagioneSociale))
            {
                f.And(x => x.RagioneSociale.Contains(model.AziendaRicercaModel_RagioneSociale));
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_MatricolaInps))
            {
                f.And(x => x.MatricolaInps == model.AziendaRicercaModel_MatricolaInps);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_CodiceFiscale))
            {
                f.And(x => x.CodiceFiscale == model.AziendaRicercaModel_CodiceFiscale);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_PartitaIva))
            {
                f.And(x => x.PartitaIva == model.AziendaRicercaModel_PartitaIva);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_CSC))
            {
                f.And(x => x.CSC == model.AziendaRicercaModel_CSC);
            }

            if (model.AziendaRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.AziendaRicercaModel_ComuneId);
            }

            if (model.AziendaRicercaModel_TipologiaId.HasValue)
            {
                f.And(x => x.TipologiaId == model.AziendaRicercaModel_TipologiaId);
            }

            if (model.AziendaRicercaModel_ConsulenteCS == "1")
            {
                f.And(x => x.ConsulenteCSId != null);
            }

            return f;
        }

        private Expression<Func<Azienda, bool>> RicercaFilter(AziendaRicercaModel model)
        {
            int? consulenteCs = null;

            if (IsUserConsulenteCs)
            {
                consulenteCs = GetConsulenteCsId.Value;
            }

            TrimAll(model);

            return x =>
            (consulenteCs != null ? (x.ConsulenteCSId == consulenteCs) : true)
            && (model.AziendaRicercaModel_RagioneSociale != null ? x.RagioneSociale.Contains(model.AziendaRicercaModel_RagioneSociale) : true
            && model.AziendaRicercaModel_MatricolaInps != null ? x.MatricolaInps == model.AziendaRicercaModel_MatricolaInps : true
            && model.AziendaRicercaModel_CodiceFiscale != null ? x.CodiceFiscale == model.AziendaRicercaModel_CodiceFiscale : true
            && model.AziendaRicercaModel_PartitaIva != null ? x.PartitaIva == model.AziendaRicercaModel_PartitaIva : true
            && model.AziendaRicercaModel_ComuneId != null ? x.ComuneId == model.AziendaRicercaModel_ComuneId : true
            && model.AziendaRicercaModel_TipologiaId != null ? x.TipologiaId == model.AziendaRicercaModel_TipologiaId : true
            && model.AziendaRicercaModel_ConsulenteCS == "1" ? x.ConsulenteCSId != null : true
            && model.AziendaRicercaModel_CSC != null ? x.CSC.Contains(model.AziendaRicercaModel_CSC) : true);
        }

        #endregion

        [AuthorizeAziendaConsulenteCs]
        public async Task<ActionResult> Dissocia(int? id = null)
        {
            try
            {
                var model = new DissociaCS();
                model.AziendaId = id;
                return AjaxView("DissociaCS", model);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "AziendaController", "Azienda"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeAziendaConsulenteCs]
        public async Task<ActionResult> Dissocia(AziendaViewModel model)
        {
            try
            {
                //var _ConsulenteCSId = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaId).FirstOrDefault().ConsulenteCSId;
                var _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaId).FirstOrDefault();
                if (_azienda == null)
                {
                    throw new Exception("Azienda non trovata");
                }

                _azienda.ConsulenteCSId = null;
                unitOfWork.AziendaRepository.Update(_azienda);

                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.AziendaId == model.AziendaId);
                if (_pratiche.Count() > 0)
                {
                    foreach (var a in _pratiche)
                    {
                        a.ConsulenteCSId = null;
                        unitOfWork.PraticheRegionaliImpreseRepository.Update(a);
                    }
                }

                var _deleghe = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(x => x.AziendaId == model.AziendaId);
                if (_deleghe.Count() > 0)
                {
                    foreach (var a in _deleghe)
                    {
                        a.DelegaAttiva = false;
                        unitOfWork.DelegheConsulenteCSAziendaRepository.Update(a);
                    }
                }

                unitOfWork.Save();
                return JsonResultTrue("Consulente dissociato!");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [AuthorizeAziendaConsulenteCs]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                var model = new AziendaViewModel();

                Expression<Func<Azienda, bool>> _filter = x => x.AziendaId == id;

                //azienda po visualizzare solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.Azienda.ToString()))
                {
                    model.MatricolaInps = User.Identity.Name;

                    _filter = x => x.MatricolaInps == model.MatricolaInps;
                }

                var _result = unitOfWork.AziendaRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    //consulente po modificare solo azienda associata a se
                    if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                    {
                        if (_result.ConsulenteCSId != GetConsulenteCsId)
                        {
                            throw new Exception("Azienda non e associata alla sua Utenza");
                        }
                    }

                    model = Reflection.CreateModel<AziendaViewModel>(_result);
                    model.DelegheConsulenteCS = _result.DelegheConsulenteCS;
                    model.Copertura = _result.Copertura;
                    model.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (User.IsInRole(IdentityHelper.Roles.Azienda.ToString()))
                    {
                        //se ruolo e azienda, e non e stato inserito ancora in tabella aziende,
                        model = new AziendaViewModel
                        {
                            MatricolaInps = _user.UserName,
                            NomeTitolare = _user.Nome,
                            CognomeTitolare = _user.Cognome,
                            Email = _user.Email,
                            InformazioniPersonaliCompilati = false
                        };
                    }
                }

                model.Tipologie = unitOfWork.TipologiaRepository.Get();

                return AjaxView("Anagrafica", model);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "AziendaController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeAziendaConsulenteCs]
        public async Task<ActionResult> Anagrafica(AziendaViewModel model)
        {
            try
            {
                if (!User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()) || model.AziendaId != 0)
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaAzienda");
                }

                //utente aggiorna solo le informazioni sua, imposta aziendaId
                if (User.IsInRole(IdentityHelper.Roles.Azienda.ToString()))
                {
                    model.MatricolaInps = User.Identity.Name;

                    Expression<Func<Azienda, bool>> _filter = x => x.MatricolaInps == model.MatricolaInps;

                    var _result = unitOfWork.AziendaRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.AziendaId = _result.AziendaId;
                        model.MatricolaInps = _result.MatricolaInps;
                    }
                }

                //check azienda exists con matricola
                var _azienda = unitOfWork.AziendaRepository.Get(x => x.MatricolaInps == model.MatricolaInps);
                if (_azienda.Where(x => x.AziendaId != model.AziendaId)?.Count() > 0)
                {
                    throw new Exception("Matricola Inps già registrata");
                }

                if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()) && model.AziendaId != 0)
                {
                    if (_azienda.FirstOrDefault().ConsulenteCSId != GetConsulenteCsId)
                    {
                        throw new Exception("Aziende non e associata alla sua Utenza");
                    }

                    model.ConsulenteCSId = GetConsulenteCsId;
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to azienda model
                var _resultModel = Reflection.CreateModel<Azienda>(model);
                _resultModel.CodiceFiscale = _resultModel.CodiceFiscale.ToUpper();
                unitOfWork.AziendaRepository.InsertOrUpdate(_resultModel);
                unitOfWork.Save();

                //aggiorna tb DelegheConsulenteCSAzienda
                if (model.AziendaId == 0 && User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                {
                    AssociaConsulenteAzienda(_resultModel.AziendaId, GetConsulenteCsId.Value, model.DelegaAzienda, model.DocumentoIdentita);
                }

                //update flag InformazioniPersonaliCompilati
                if (User.IsInRole(IdentityHelper.Roles.Azienda.ToString()))
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
                    aziendaId = _resultModel.AziendaId,
                    message = "Anagrafica " + (model.AziendaId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        #region associa azienda a consulente / cs

        [AuthorizeConsulenteCs]
        public ActionResult AssociaAziendaRicerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [AuthorizeConsulenteCs]
        public async Task<ActionResult> AssociaAziendaRicerca(AziendaAssociaRicercaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                int? consulenteCs = GetConsulenteCsId;

                Expression<Func<Azienda, bool>> _filter = x =>
                (consulenteCs != null ? (x.ConsulenteCSId != consulenteCs) : true)
                && ((x.AziendaId == model.AziendaAssociaRicercaModel_AziendaId));

                var _result = await unitOfWork.AziendaRepository.GetAsync(_filter);
                if (_result.Count() == 0)
                {
                    return JsonResultFalse("Nessuna Azienda trovata.");
                }
                AziendaAssociaRicercaViewModel aziendaAssociaRicercaViewModel = new AziendaAssociaRicercaViewModel();
                aziendaAssociaRicercaViewModel.Aziende = _result;
                return AjaxView("AssociaAziendaRicercaList", aziendaAssociaRicercaViewModel);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [AuthorizeConsulenteCs]
        public ActionResult AssociaAzienda(AziendaAssociaRicercaViewModel model)
        {
            try
            {
                AssociaConsulenteAzienda(model.AziendaId, GetConsulenteCsId.Value, model.DelegaAzienda, model.DocumentoIdentita);
                return JsonResultTrue("Azienda associata");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        void AssociaConsulenteAzienda(int aziendaId, int consulenteCSId, string delegaAzienda, string documentoIdentita)
        {
            try
            {

                var _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == aziendaId).FirstOrDefault();

                if (_azienda == null)
                {
                    throw new Exception("Azienda non trovata");
                }

                //aggiorna tb DelegheConsulenteCSAzienda
                var _delegaattiva = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(xx => xx.DelegaAttiva == true
                && xx.AziendaId == aziendaId);

                if (_delegaattiva != null)
                {
                    foreach (var item in _delegaattiva)
                    {
                        item.DataDelegaDisdetta = DateTime.Now;
                        item.DelegaAttiva = false;
                        unitOfWork.DelegheConsulenteCSAziendaRepository.Update(item);
                    }
                }

                //inserisci 
                var _consulenteCsId = GetConsulenteCsId.GetValueOrDefault();

                DelegheConsulenteCSAzienda _delega = new DelegheConsulenteCSAzienda
                {
                    DelegaAttiva = true,
                    AziendaId = aziendaId,
                    ConsulenteCSId = GetConsulenteCsId.GetValueOrDefault(),
                    DataInserimento = DateTime.Now,
                    DelegaAzienda = Savefile(GetUploadFolder(PathDelegheAzienda, _consulenteCsId), delegaAzienda),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, _consulenteCsId), documentoIdentita),
                };

                unitOfWork.DelegheConsulenteCSAziendaRepository.Insert(_delega);

                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.AziendaId == aziendaId);
                if (_pratiche.Count() > 0)
                {
                    foreach (var a in _pratiche)
                    {
                        a.ConsulenteCSId = consulenteCSId;
                        unitOfWork.PraticheRegionaliImpreseRepository.Update(a);
                    }
                }

                //aggiorna ConsulenteCsId tabella aienda
                _azienda.ConsulenteCSId = consulenteCSId;

                unitOfWork.Save(false);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [AuthorizeAziendaConsulenteCs]
        public ActionResult UploadAllegato(int delegaId, string allegato)
        {
            var _delega = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(x => x.DelegheConsulenteCSAziendaId == delegaId).FirstOrDefault();

            if (_delega == null)
            {
                throw new Exception("Delega non trovata");
            }

            AziendaUploadAllegatoModel model = new AziendaUploadAllegatoModel();
            model.TipoAllegato = allegato;
            model.DelegheConsulenteCSAziendaId = delegaId;
            model.AziendaId = _delega.AziendaId;

            return AjaxView("UploadAllegato", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeAziendaConsulenteCs]
        public ActionResult UploadAllegato(AziendaUploadAllegatoModel model)
        {
            try
            {
                var _delega = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(x => x.DelegheConsulenteCSAziendaId == model.DelegheConsulenteCSAziendaId).FirstOrDefault();

                if (_delega == null)
                {
                    throw new Exception("Delega non trovata");
                }

                if (IsUserConsulenteCs)
                {
                    if (_delega.ConsulenteCSId != GetConsulenteCsId.Value)
                    {
                        throw new Exception("Richiesta non valida");
                    }
                }

                if (model.TipoAllegato == "DocumentoIdentita")
                {
                    _delega.DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, _delega.ConsulenteCSId), model.Allegato);
                }

                if (model.TipoAllegato == "DelegaAzienda")
                {
                    _delega.DelegaAzienda = Savefile(GetUploadFolder(PathDelegheAzienda, _delega.ConsulenteCSId), model.Allegato);
                }

                unitOfWork.DelegheConsulenteCSAziendaRepository.Update(_delega);
                unitOfWork.Save(false);

                return JsonResultTrue("Allegato aggiornato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public JsonResult ListaAziendeSportello(string phrase, int dipendenteId)
        {
            var aziendeIds = new List<int>();

            aziendeIds = unitOfWork.DipendenteAziendaRepository.Get((x => ((x.DipendenteId == dipendenteId)
            && (x.DataCessazione == null)
            && (x.Dipendente.SportelloId == (int)GetSportelloId.Value))))?.Select(x => x.AziendaId)?.ToList();

            Expression<Func<Azienda, bool>> _filter = x =>
            (aziendeIds.Contains(x.AziendaId))
            && ((phrase != null ? x.RagioneSociale.Contains(phrase) : true)
            || (phrase != null ? x.MatricolaInps.Contains(phrase) : true));

            return GetListaAziende(_filter);
        }

        public JsonResult ListaAziende(string phrase)
        {
            var aziendeIds = new List<int>();

            if (IsUserDipendente)
            {
                aziendeIds = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == (int)GetDipendenteId.Value && x.DataCessazione == null)?.Select(x => x.AziendaId)?.ToList();
            }

            Expression<Func<Azienda, bool>> _filter = x =>
            ((IsUserConsulenteCs ? (x.ConsulenteCSId == (int)GetConsulenteCsId.Value) : true)
            && (IsUserDipendente ? (aziendeIds.Contains(x.AziendaId)) : true))
            && ((phrase != null ? x.RagioneSociale.Contains(phrase) : true)
            || (phrase != null ? x.MatricolaInps.Contains(phrase) : true));

            return GetListaAziende(_filter);
        }

        public JsonResult ListaAziendeAll(string phrase)
        {
            var aziendeIds = new List<int>();

            Expression<Func<Azienda, bool>> _filter = x =>
            ((phrase != null ? x.RagioneSociale.Contains(phrase) : true)
            || (phrase != null ? x.MatricolaInps.StartsWith(phrase) || x.MatricolaInps.Contains(phrase) : true));

            return GetListaAziende(_filter);

        }

        private JsonResult GetListaAziende(Expression<Func<Azienda, bool>> filter)
        {
            var _result = unitOfWork.AziendaRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.RagioneSociale == null || p.RagioneSociale == "")
                       .ThenBy(p => p.RagioneSociale)
                       .Select(x => new { x.AziendaId, x.MatricolaInps, RagioneSociale = x.RagioneSociale + " - " + x.MatricolaInps }).Distinct(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadAllegato(int? delegaId, string allegato)
        {
            try
            {
                var _allegato = unitOfWork.DelegheConsulenteCSAziendaRepository.Get(x => x.DelegheConsulenteCSAziendaId == delegaId).FirstOrDefault();

                var _uploadFolder = "";

                var _file = allegato == "DelegaAzienda" ? _allegato?.DelegaAzienda : _allegato?.DocumentoIdentita;

                if (allegato == "DelegaAzienda")
                {
                    _uploadFolder = GetUploadFolder(PathDelegheAzienda, _allegato.ConsulenteCSId);
                }
                else
                {
                    _uploadFolder = GetUploadFolder(PathCartaIdentita, _allegato.ConsulenteCSId);
                }

                if (_allegato == null || _file  == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _file)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = System.Web.MimeMapping.GetMimeMapping(_file);
                return File(Path.Combine(_uploadFolder, _file), mimeType, $"{allegato}{Path.GetExtension(_file)}");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
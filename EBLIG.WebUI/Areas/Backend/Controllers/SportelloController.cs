using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class SportelloController : BaseController
    {
        #region ricerca

        [AuthorizeAdmin]
        public ActionResult Ricerca()
        {
            return AjaxView("Ricerca");
        }

        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult Ricerca(SportelloRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.SportelloRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);
            //var _query = unitOfWork.SportelloRepository.Get(RicercaFilter(model));

            var _result = GeModelWithPaging<SportelloRicercaViewModel, Sportello>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [AuthorizeAdmin]
        public ActionResult RicercaExcel(SportelloRicercaModel model)
        {
            var _query = from a in unitOfWork.SportelloRepository.Get(RicercaFilter(model)).OrderBy(r => r.RagioneSociale)
                         select new
                         {
                             a.RagioneSociale,
                             a.CodiceFiscalePIva,
                             a.Cognome,
                             a.Nome,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Sportello");
        }

        private Expression<Func<Sportello, bool>> RicercaFilter(SportelloRicercaModel model)
        {
            TrimAll(model);

            return x =>
            (model.SportelloRicercaModel_RagioneSociale != null ? x.RagioneSociale.Contains(model.SportelloRicercaModel_RagioneSociale) : true
            && model.SportelloRicercaModel_CodiceFiscalePartitaIva != null ? x.CodiceFiscalePIva == model.SportelloRicercaModel_CodiceFiscalePartitaIva : true
            && model.SportelloRicercaModel_ComuneId != null ? x.ComuneId == model.SportelloRicercaModel_ComuneId : true);
        }

        private SqlLam<Sportello> RicercaFilter2(SportelloRicercaModel model)
        {
            TrimAll(model);

            var f = new SqlLam<Sportello>();

            if (!string.IsNullOrWhiteSpace(model.SportelloRicercaModel_RagioneSociale))
            {
                f.And(xd => xd.RagioneSociale.Contains(model.SportelloRicercaModel_RagioneSociale));
            }

            if (!string.IsNullOrWhiteSpace(model.SportelloRicercaModel_CodiceFiscalePartitaIva))
            {
                f.And(xd => xd.CodiceFiscalePIva.Contains(model.SportelloRicercaModel_CodiceFiscalePartitaIva));
            }

            if (model.SportelloRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.SportelloRicercaModel_ComuneId);
            }

            return f.OrderBy(d => d.Cellulare);
        }

        #endregion

        [AuthorizeSportello]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                Expression<Func<Sportello, bool>> _filter = x => x.SportelloId == id;

                //utente visualizza solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                {
                    _filter = x => x.CodiceFiscalePIva == User.Identity.Name;
                }

                var _outModel = new SportelloViewModel();

                var _result = unitOfWork.SportelloRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    _outModel = Reflection.CreateModel<SportelloViewModel>(_result);
                    _outModel.DelegheSportelloDipendente = _result.DelegheSportelloDipendente;
                    _outModel.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                    {
                        _outModel = new SportelloViewModel
                        {
                            CodiceFiscalePIva = _user.UserName,
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
                return AjaxView("Error", new HandleErrorInfo(ex, "SportelloController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeSportello]
        public async Task<ActionResult> Anagrafica(SportelloViewModel model)
        {
            try
            {

                //utente aggiorna solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                {
                    model.CodiceFiscalePIva = User.Identity.Name;

                    Expression<Func<Sportello, bool>> _filter = x => x.CodiceFiscalePIva == User.Identity.Name;

                    var _result = unitOfWork.SportelloRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.SportelloId = _result.SportelloId;
                        model.CodiceFiscalePIva = _result.CodiceFiscalePIva;
                    }
                }

                //check consulente exists con codice fiscale
                var _sportelloExists = unitOfWork.SportelloRepository.Get(x => x.SportelloId != model.SportelloId && x.CodiceFiscalePIva.ToLower() == model.CodiceFiscalePIva.ToLower());
                if (_sportelloExists?.Count() > 0)
                {
                    throw new Exception("Codice Fiscale / Partita Iva già registrata");
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to consulenteCs model
                var _model = Reflection.CreateModel<Sportello>(model);
                _model.CodiceFiscalePIva = _model.CodiceFiscalePIva.ToUpper();
                unitOfWork.SportelloRepository.InsertOrUpdate(_model);
                unitOfWork.Save();

                //update flag InformazioniPersonaliCompilati
                if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
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
                    sportelloId = _model.SportelloId,
                    message = "Anagrafica " + (model.SportelloId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
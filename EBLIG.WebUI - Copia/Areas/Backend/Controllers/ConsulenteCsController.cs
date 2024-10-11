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
    public class ConsulenteCsController : BaseController
    {
        #region ricerca

        [AuthorizeAdmin]
        public ActionResult Ricerca()
        {
            return AjaxView("Ricerca");
        }

        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult Ricerca(ConsulenteCsRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.ConsulenteCSRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);
            //var _query = unitOfWork.ConsulenteCSRepository.Get(RicercaFilter(model));

            var _result = GeModelWithPaging<ConsulenteCsRicercaViewModel, ConsulenteCS>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [AuthorizeAdmin]
        public ActionResult RicercaExcel(ConsulenteCsRicercaModel model)
        {
            var _query = from a in unitOfWork.ConsulenteCSRepository.Get(RicercaFilter(model)).OrderBy(r => r.RagioneSociale)
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
            return _excel.CreateExcel(_query, "ConsulentiCs");
        }

        private Expression<Func<ConsulenteCS, bool>> RicercaFilter(ConsulenteCsRicercaModel model)
        {
            TrimAll(model);

            return x =>
            (model.ConsulenteCsRicercaModel_RagioneSociale != null ? x.RagioneSociale.Contains(model.ConsulenteCsRicercaModel_RagioneSociale) : true
            && model.ConsulenteCsRicercaModel_CodiceFiscalePartitaIva != null ? x.CodiceFiscalePIva == model.ConsulenteCsRicercaModel_CodiceFiscalePartitaIva : true
            && model.ConsulenteCsRicercaModel_ComuneId != null ? x.ComuneId == model.ConsulenteCsRicercaModel_ComuneId : true);
        }

        private SqlLam<ConsulenteCS> RicercaFilter2(ConsulenteCsRicercaModel model)
        {
            TrimAll(model);

            var f = new SqlLam<ConsulenteCS>();

            if (!string.IsNullOrWhiteSpace(model.ConsulenteCsRicercaModel_RagioneSociale))
            {
                f.And(xd => xd.RagioneSociale.Contains(model.ConsulenteCsRicercaModel_RagioneSociale));
            }

            if (!string.IsNullOrWhiteSpace(model.ConsulenteCsRicercaModel_CodiceFiscalePartitaIva))
            {
                f.And(xd => xd.CodiceFiscalePIva.Contains(model.ConsulenteCsRicercaModel_CodiceFiscalePartitaIva));
            }

            if (model.ConsulenteCsRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.ConsulenteCsRicercaModel_ComuneId);
            }

            return f.OrderBy(d => d.Cellulare);
        }

        #endregion

        [AuthorizeConsulenteCs]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                Expression<Func<ConsulenteCS, bool>> _filter = x => x.ConsulenteCSId == id;

                //utente visualizza solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                {
                    _filter = x => x.CodiceFiscalePIva == User.Identity.Name;
                }

                var _outModel = new ConsulenteCSViewModel();

                var _result = unitOfWork.ConsulenteCSRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    _outModel = Reflection.CreateModel<ConsulenteCSViewModel>(_result);
                    _outModel.DelegheConsulenteCS = _result.DelegheConsulenteCS;
                    _outModel.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                    {
                        _outModel = new ConsulenteCSViewModel
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
                return AjaxView("Error", new HandleErrorInfo(ex, "COnsulenteCsController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeConsulenteCs]
        public async Task<ActionResult> Anagrafica(ConsulenteCSViewModel model)
        {
            try
            {

                //utente aggiorna solo le informazioni sua
                if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                {
                    model.CodiceFiscalePIva = User.Identity.Name;

                    Expression<Func<ConsulenteCS, bool>> _filter = x => x.CodiceFiscalePIva == User.Identity.Name;

                    var _result = unitOfWork.ConsulenteCSRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.ConsulenteCSId = _result.ConsulenteCSId;
                        model.CodiceFiscalePIva = _result.CodiceFiscalePIva;
                    }
                }

                //check consulente exists con codice fiscale
                var _consulenteCsExists = unitOfWork.ConsulenteCSRepository.Get(x => x.ConsulenteCSId != model.ConsulenteCSId && x.CodiceFiscalePIva.ToLower() == model.CodiceFiscalePIva.ToLower());
                if (_consulenteCsExists?.Count() > 0)
                {
                    throw new Exception("Codice Fiscale / Partita Iva già registrata");
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to consulenteCs model
                var _model = Reflection.CreateModel<ConsulenteCS>(model);
                _model.CodiceFiscalePIva = _model.CodiceFiscalePIva.ToUpper();
                unitOfWork.ConsulenteCSRepository.InsertOrUpdate(_model);
                unitOfWork.Save();

                //update flag InformazioniPersonaliCompilati
                if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
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
                    consulenteCSId = _model.ConsulenteCSId,
                    message = "Anagrafica " + (model.ConsulenteCSId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
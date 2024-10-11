using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using EBLIG.DOM;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Models;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Helpers;
using EBLIG.WebUI.Hubs;
using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    public class UtentiController : BaseController
    {
        [AuthorizeAdmin]
        public ActionResult Ricerca()
        {
            UtentiRicercaModel model = new UtentiRicercaModel
            {
                Ruoli = GenericHelper.GetRolesFriendlyName(null),
            };

            return AjaxView("Ricerca", model);
        }

        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult Ricerca(UtentiRicercaModel model, int? page)
        {
            try
            {
                var _roles = RoleManager.Roles;

                var _query = UserManager.Users.Where(RicercaFilter(model)).Select(x => new UtentiViewModel
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Cognome = x.Cognome,
                    Email = x.Email,
                    Nome = x.Nome,
                    EmailConfermata = x.EmailConfirmed,
                    RuoloId = x.Roles.FirstOrDefault().RoleId,
                    Bloccato = x.LockoutEndDateUtc != null
                });

                var _result = GeModelWithPaging<UtentiRicercaViewModel, UtentiViewModel>(page, _query.AsEnumerable(), model, 10);

                var _ruolo = GenericHelper.GetRolesFriendlyName(null);

                foreach (var item in _result.Result)
                {
                    //item.Ruolo = _roles.FirstOrDefault(c => c.Id == item.RuoloId).Name;
                    item.Ruolo = _ruolo.FirstOrDefault(x => x.Item1 == item.RuoloId).Item3;
                    item.RuoloFriendlyName = item.Ruolo;
                }

                return AjaxView("RicercaList", _result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AuthorizeAdmin]
        public ActionResult UtenteNuovo(string tipo)
        {
            try
            {
                UtentiViewModel _user = new UtentiViewModel();

                var _ruolo = GenericHelper.GetRolesFriendlyName(null).FirstOrDefault(x => x.Item2 == tipo);
                _user.Ruolo = _ruolo.Item2;
                _user.RuoloFriendlyName = _ruolo.Item3;

                return AjaxView("Utente", _user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private string GetRuolo(string tipo)
        //{
        //    switch (Enum.Parse(typeof(IdentityHelper.Roles), tipo, true))
        //    {
        //        case IdentityHelper.Roles.Admin:
        //            return IdentityHelper.Roles.Admin.ToString();
        //        case IdentityHelper.Roles.Sportello:
        //            return IdentityHelper.Roles.Sportello.ToString();
        //        default:
        //            throw new Exception("Ruolo non valido");
        //    }
        //}

        [AuthorizeAdmin]
        public ActionResult Utente(string id)
        {
            try
            {
                UtentiViewModel _user = UserManager.Users.Where(x => x.Id == id).Select(x => new UtentiViewModel
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Cognome = x.Cognome,
                    Nome = x.Nome,
                    Email = x.Email,
                    RuoloId = x.Roles.FirstOrDefault().RoleId,
                    Bloccato = x.LockoutEndDateUtc != null,
                    EmailConfermata = x.EmailConfirmed,
                    Ruolo = RoleManager.Roles.FirstOrDefault(c => c.Id == x.Roles.FirstOrDefault().RoleId).Name,
                    //RuoloFriendlyName = GenericHelper.GetRoleFriendlyName(null, RoleManager.Roles.FirstOrDefault(c => c.Id == x.Roles.FirstOrDefault().RoleId).Name)


                }).FirstOrDefault();

                return AjaxView("Utente", _user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AuthorizeAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Utente(UtentiViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserId))
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                }

                //update user
                if (!string.IsNullOrWhiteSpace(model.UserId))
                {
                    var _user = UserManager.Users.FirstOrDefault(x => x.Id == model.UserId);

                    if (_user == null)
                    {
                        throw new Exception("Utente non trovato");
                    }

                    _user.Nome = model.Nome;
                    _user.Email = model.Email;
                    _user.Cognome = model.Cognome;

                    if (!model.Bloccato.GetValueOrDefault())
                    {
                        _user.LockoutEndDateUtc = null;
                    }
                    else
                    {
                        _user.LockoutEndDateUtc = DateTime.MaxValue;
                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
                        context.Clients.All.onLogOffUtente(model.UserName, "Attenzione, sei stato bloccato dal Amministratore del sistema");
                    }

                    await UserManager.UpdateAsync(_user);

                    return JsonResultTrue("Utente aggiornato");
                }
                else
                {
                    var _ruolo = GenericHelper.GetRolesFriendlyName(null).FirstOrDefault(x => x.Item2 == model.Ruolo);

                    //crea utente
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Cognome = model.Cognome,
                        Nome = model.Nome,
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        RoleManager.Create(new IdentityRole
                        {
                            Name = _ruolo.Item2
                        });

                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        UserManager.AddToRole(user.Id, _ruolo.Item2);

                        // Per altre informazioni su come abilitare la conferma dell'account e la reimpostazione della password, vedere https://go.microsoft.com/fwlink/?LinkID=320771
                        // Inviare un messaggio di posta elettronica con questo collegamento
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        //var callbackUrl = Url.Action("ConfirmEmail", "Registrazione", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                        NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                        c.Add("userId", user.Id);
                        c.Add("code", code);

                        var callbackUrl = $"{UriPortale("ConfirmEmail", "Registrazione")}?{c.ToString()}";

                        RegistrazioneConfermaModel _resultModel = new RegistrazioneConfermaModel
                        {
                            UrlConferma = callbackUrl,
                            Email = model.Email,
                            Cognome = model.Cognome,
                            Nome = model.Nome,
                            Username = model.UserName
                        };

                        await UserManager.SendEmailAsync(user.Id, "Conferma account", RenderTemplate("Registrazione/Confirm_Mail", _resultModel));

                        return JsonResultTrue("Utente creato");
                    }
                    else
                    {
                        return JsonResultFalse(ErrorsToString(result.Errors));
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [AuthorizeAdmin]
        public ActionResult RicercaExcel(UtentiRicercaModel model)
        {
            var _query = UserManager.Users.Where(RicercaFilter(model)).Select(x => new
            {
                x.UserName,
                x.Cognome,
                x.Email,
                x.Nome,
                Ruolo = RoleManager.Roles.FirstOrDefault(m => m.Id == x.Roles.FirstOrDefault().RoleId).Name,
                EmailConfermata = x.EmailConfirmed ? "Si" : "No",
                Bloccato = x.LockoutEndDateUtc != null ? "Si" : "No"
            });

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Utenti");
        }

        [AuthorizeAdmin]
        [HttpPost]
        public async Task<ActionResult> EliminaUtente(string id)
        {
            try
            {
                var _user = UserManager.FindById(id);
                var _username = _user.UserName;
                var _result = await UserManager.DeleteAsync(_user);

                if (_result == IdentityResult.Success)
                {
                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
                    context.Clients.All.onLogOffUtente(_username, "Attenzione, la tua utenza e stata eliminata dal Amministratore del sistema");
                    return JsonResultTrue("Utente eliminato");
                }

                return JsonResultFalse(ErrorsToString(_result.Errors));

            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [AuthorizeAdmin]
        private Expression<Func<ApplicationUser, bool>> RicercaFilter(UtentiRicercaModel model)
        {
            TrimAll(model);

            return x => model.UtentiRicercaModel_Username != null ? x.UserName.ToUpper().Contains(model.UtentiRicercaModel_Username.ToUpper()) : true
            && model.UtentiRicercaModel_RuoId != null ? x.Roles.FirstOrDefault().RoleId == model.UtentiRicercaModel_RuoId : true
            && (model.UtentiRicercaModel_Email != null ? x.Email.Contains(model.UtentiRicercaModel_Email) : true)
            && model.UtentiRicercaModel_EmailConfermata != null ? (model.UtentiRicercaModel_EmailConfermata == "1" ? x.EmailConfirmed : !x.EmailConfirmed) : true
            && model.UtentiRicercaModel_Bloccato != null ? (model.UtentiRicercaModel_Bloccato == "1" ? x.LockoutEndDateUtc != null : x.LockoutEndDateUtc == null) : true;
        }

        public ActionResult ModificaPassword()
        {
            return AjaxView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaPassword(UtentiModificaPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelStateErrorToString(ModelState));
            }

            var _user = UserManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            if (_user == null)
            {
                throw new Exception("Utente non valido");
            }

            var _result = UserManager.ChangePassword(_user.Id, model.PasswordVecchia, model.PasswordNuova);

            if (_result.Succeeded)
            {
                return JsonResultTrue("Password cambiata");
            }
            else
            {
                return JsonResultFalse(ErrorsToString(_result.Errors));
            }
        }
        public JsonResult ListaUtenti(string phrase)
        {
            Expression<Func<Utente, bool>> _filter = x =>
            (phrase != null ? (x.Email).Contains(phrase) : true);

            return GetListaUtenti(_filter);
        }

        private JsonResult GetListaUtenti(Expression<Func<Utente, bool>> filter)
        {
            var _result = unitOfWork.UtentiRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Email == null || p.Email == "")
                       .Select(x => new { x.UserName, x.Email, x.Cognome, x.Nome, Nominativo = x.Cognome + " " + x.Nome }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


    }
}
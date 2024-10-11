using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Controllers
{
    public class RegistrazioneController : BaseController
    {
        public ActionResult Azienda()
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect("~/");
            }

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Azienda(RegistrazioneAzienda model)
        {
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/");
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                return await CreateUser(model.MatricolaInps, model.Nome, model.Cognome, model.Email, model.Password, IdentityHelper.Roles.Azienda.ToString());
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult ConsulenteCs()
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect("~/");
            }

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConsulenteCs(RegistrazioneConsulenteCs model)
        {
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/");
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                return await CreateUser(model.CodiceFiscalePIva, model.Nome, model.Cognome, model.Email, model.Password, IdentityHelper.Roles.ConsulenteCs.ToString());
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Dipendente()
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect("~/");
            }

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Dipendente(RegistrazioneDipendente model)
        {
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/");
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                return await CreateUser(model.CodiceFiscale, model.Nome, model.Cognome, model.Email, model.Password, IdentityHelper.Roles.Dipendente.ToString());
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        private async Task<ActionResult> CreateUser(string username, string nome, string cognome, string email, string password, string ruolo)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    Cognome = cognome,
                    Nome = nome,
                };

                var result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    RoleManager.Create(new IdentityRole
                    {
                        Name = ruolo
                    });

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    UserManager.AddToRole(user.Id, ruolo);

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
                        Email = email,
                        Cognome = cognome,
                        Nome = nome,
                        Username = username
                    };

                    await UserManager.SendEmailAsync(user.Id, "Conferma account", RenderTemplate("Registrazione/Confirm_Mail", _resultModel));

                    return JsonResultTrue(RenderTemplate("Registrazione/Confirm", _resultModel));
                }
                else
                {
                    return JsonResultFalse(ErrorsToString(result.Errors));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return View("Error");
                }
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
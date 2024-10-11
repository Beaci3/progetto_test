using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Threading;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;

namespace EBLIG.WebUI.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Login(string returnUrl)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();

            ViewBag.ReturnUrl = returnUrl;
            return AjaxView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var user = UserManager.FindByName(model.Username);

                if (user == null)
                {
                    throw new Exception("Username o Password non validi");
                }

                await IsEmailConfirmed(user);

                //commento di leila
                //commento bea

                // Questa opzione non calcola il numero di tentativi di accesso non riusciti per il blocco dell'account
                // Per abilitare il conteggio degli errori di password per attivare il blocco, impostare shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, shouldLockout: true);

                if (result == SignInStatus.LockedOut)
                {
                    throw new Exception("Utente bloccato, accesso negato");
                }

                if (result != SignInStatus.Success)
                {
                    throw new Exception("Username o Password non validi");
                }

                var url = Url.Action("Index", "Home", new { area = "Backend" });

                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        isValid = true,
                        message = "Utente autorizzato. Attendere, redirect in corso...",
                        redirectUrl = url
                    }, JsonRequestBehavior.AllowGet);
                }

                return Redirect(url);


            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return JsonResultFalse(ex.Message);
                }

                TempData["message"] = ex.Message;
                return AjaxView("Login", model);
            }
        }

        public ActionResult ForgotPassword()
        {
            return AjaxView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    throw new Exception("Indirizzo email non trovato");
                }

                if (user.LockoutEndDateUtc != null)
                {
                    throw new Exception("Utente bloccato, non possibile recuperare la password");
                }

                await IsEmailConfirmed(user);

                // Inviare un messaggio di posta elettronica con questo collegamento
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                c.Add("userId", user.Id);
                c.Add("code", code);

                var callbackUrl = $"{UriPortale("ResetPassword", "Account")}?{c.ToString()}";

                RecuperoPasswordConfermaModel _resultModel = new RecuperoPasswordConfermaModel
                {
                    UrlConferma = callbackUrl,
                    Email = user.Email,
                    Cognome = user.Cognome,
                    Nome = user.Nome,
                    Username = user.UserName
                };

                await UserManager.SendEmailAsync(user.Id, "Recupero Password", RenderTemplate("Account/ForgotPassword_Mail", _resultModel));

                var _html = RenderTemplate("Account/ForgotPassword", _resultModel);

                if (Request.IsAjaxRequest())
                {
                    return JsonResultTrue(_html);
                }

                TempData["message"] = _html;
                return AjaxView("ForgotPassword", model);

            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return JsonResultFalse(ex.Message);
                }

                TempData["message"] = ex.Message;
                return AjaxView("RecuperoPwd", model);
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var user = await UserManager.FindByNameAsync(model.Username);

                var url = Url.Action("ResetPasswordConfirmation", "Account");

                if (user == null)
                {
                    // Non rivelare che l'utente non esiste
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new
                        {
                            isValid = true,
                            message = "Utente autorizzato. Attendere, redirect in corso...",
                            redirectUrl = url
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return Redirect(url);
                }

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (!result.Succeeded)
                {
                    throw new Exception(ErrorsToString(result.Errors));
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        isValid = true,
                        redirectUrl = url
                    }, JsonRequestBehavior.AllowGet);
                }

                return Redirect(url);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return JsonResultFalse(ex.Message);
                }

                TempData["message"] = ex.Message;
                return AjaxView("ResetPassword", model);
            }
        }

        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        ///  verifica indirizzo email e confermato, se no, invia nuovo codice
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="Exception"></exception>
        private async Task IsEmailConfirmed(ApplicationUser user)
        {
            if (!UserManager.IsEmailConfirmed(user.Id))
            {
                string code = UserManager.GenerateEmailConfirmationToken(user.Id);

                // var callbackUrl = Url.Action("ConfirmEmail", "Registrazione", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                c.Add("userId", user.Id);
                c.Add("code", code);

                var callbackUrl = $"{UriPortale("ConfirmEmail", "Registrazione")}?{c.ToString()}";


                RegistrazioneConfermaModel _resultModel = new RegistrazioneConfermaModel
                {
                    UrlConferma = callbackUrl,
                    Email = user.Email,
                    Cognome = user.Cognome,
                    Nome = user.Nome,
                    Username = user.UserName
                };

                await UserManager.SendEmailAsync(user.Id, "Conferma account", RenderTemplate("Registrazione/Confirm_Mail", _resultModel));

                var _html = RenderTemplate("Registrazione/Confirm", _resultModel);

                throw new Exception(_html);
            }
        }

        public ActionResult LogOff()
        {
            if (User != null)
            {
                UserOnlineAttribute.RemoveUser(User.Identity.Name);
            }

            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            HttpContext.Response.Cookies.Remove("AspNet.ApplicationCookie");

            Thread.Sleep(1500);
            return RedirectToAction("Index", "Home");
        }
    }
}
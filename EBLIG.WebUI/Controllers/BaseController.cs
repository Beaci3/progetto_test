using EBLIG.DOM;
using EBLIG.DOM.DAL;
using EBLIG.Utils;
using EBLIG.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MimeKit;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Controllers
{
    public class BaseController : Controller
    {
        public bool IsUserAdmin
        {
            get
            {
                return User.IsInRole(IdentityHelper.Roles.Admin.ToString());
            }
            set { }
        }

        public bool IsUserAzienda
        {
            get
            {
                return User.IsInRole(IdentityHelper.Roles.Azienda.ToString());
            }
            set { }
        }

        public bool IsUserDipendente
        {
            get
            {
                return User.IsInRole(IdentityHelper.Roles.Dipendente.ToString());
            }
            set { }
        }
        public bool IsUserConsulenteCs
        {
            get
            {
                return User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString());
            }
            set { }
        }

        public bool IsUserSportello
        {
            get
            {
                return User.IsInRole(IdentityHelper.Roles.Sportello.ToString());
            }
            set { }
        }

        public int? GetAziendaId
        {
            get
            {
                if (User.IsInRole(IdentityHelper.Roles.Azienda.ToString()))
                {
                    UnitOfWork _unitOfWork = new UnitOfWork();
                    return _unitOfWork.AziendaRepository.Get(c => c.MatricolaInps.ToLower() == User.Identity.Name.ToLower()).FirstOrDefault().AziendaId;
                }
                return null;
            }
            set { }
        }

        public int? GetSportelloId
        {
            get
            {
                if (User.IsInRole(IdentityHelper.Roles.Sportello.ToString()))
                {
                    UnitOfWork _unitOfWork = new UnitOfWork();
                    return _unitOfWork.SportelloRepository.Get(c => c.CodiceFiscalePIva.ToLower() == User.Identity.Name.ToLower()).FirstOrDefault().SportelloId;
                }
                return null;
            }
            set { }
        }

        public int? GetConsulenteCsId
        {
            get
            {
                if (User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()))
                {
                    UnitOfWork _unitOfWork = new UnitOfWork();
                    return _unitOfWork.ConsulenteCSRepository.Get(c => c.CodiceFiscalePIva.ToLower() == User.Identity.Name.ToLower()).FirstOrDefault().ConsulenteCSId;
                }
                return null;
            }
            set { }
        }

        public int? GetDipendenteId
        {
            get
            {
                if (User.IsInRole(IdentityHelper.Roles.Dipendente.ToString()))
                {
                    UnitOfWork _unitOfWork = new UnitOfWork();
                    return _unitOfWork.DipendenteRepository.Get(c => c.CodiceFiscale.ToLower() == User.Identity.Name.ToLower()).FirstOrDefault().DipendenteId;
                }
                return null;
            }
            set { }
        }

        public string UriPortale(string action, string contoller)
        {
            return $"{ConfigurationProvider.Instance.GetConfiguration().RagioneSociale.UriPortale}/{contoller}/{action}";
        }

        public string GetUploadFolder(string path, int p)
        {
            path = string.Format(path, p.ToString().PadLeft(7, '0'));
            var cartellaServer = Path.Combine(ConfigurationProvider.Instance.GetConfiguration().UploadFolder, path);
           // cartellaServer = Path.Combine(cartellaServer, );

            if (!Directory.Exists(cartellaServer))
            {
                Directory.CreateDirectory(cartellaServer);
            }

            return cartellaServer;
        }

        public ActionResult DownloadAllegatoFromServer(string path, string allegato)
        {
            try
            {
                if (!System.IO.File.Exists(Path.Combine(path, allegato)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = MimeMapping.GetMimeMapping(allegato);
                return File(Path.Combine(path, allegato), mimeType, allegato + Path.GetExtension(allegato));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// torna nome del file salvato
        /// </summary>
        /// <param name="cartellaServer"></param>
        /// <param name="base64"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string Savefile(string cartellaServer, string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            if (!Directory.Exists(cartellaServer))
            {
                Directory.CreateDirectory(cartellaServer);
            }

            var mimetype = base64?.Split(new string[] { "data:" }, StringSplitOptions.None);

            mimetype = mimetype?.LastOrDefault()?.Split(';');

            var extension = MimeTypes.MimeTypeMap.GetExtension(mimetype?.FirstOrDefault());

            var filename = $"{Guid.NewGuid()}{extension}";

            byte[] filetosave = Convert.FromBase64String(base64.Split(',').LastOrDefault());

            System.IO.File.WriteAllBytes(Path.Combine(cartellaServer, filename), filetosave);

            return filename;
        }

        public UnitOfWork unitOfWork = new UnitOfWork();

        public string RenderTemplate(string template, object model, HttpContext context = null)
        {
            return PartialView($"~/Views/Template/{template}.cshtml").RenderViewToString(model, context);
        }

        public List<string> IsValidModel(object[] value)
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

        #region Membership helper

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public BaseController()
        {
        }

        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                try
                {
                    return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                catch
                {
                    return null;
                }
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                try
                {
                    return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                catch
                {
                    return null;
                }
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                try
                {
                    return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                }
                catch
                {
                    return null;
                }
            }
            private set
            {
                _roleManager = value;
            }
        }

        public List<string> GetUserRoles()
        {
            return UserManager.GetRoles(User.Identity.GetUserId()).ToList();
        }

        public string GetUserRole()
        {
            try
            {
                if (User != null)
                {
                    return UserManager.GetRoles(User.Identity.GetUserId()).FirstOrDefault();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        #endregion

        #region Json helper

        public JsonResult JsonResultTrue(string message)
        {
            return Json(new
            {
                isValid = true,
                message,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResultFalse(string message)
        {
            return Json(new
            {
                isValid = false,
                message,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region mvc helper

        public ActionResult AjaxView(string viename = null, object model = null)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView(viename, model);
                }
                else
                {
                    return View(viename, model);
                }

            }
            catch (Exception)
            {
                return View(viename, model);
            }
        }

        internal string ModelStateErrorToString(ModelStateDictionary modelState)
        {
            try
            {
                StringBuilder esito = new StringBuilder();

                foreach (var item in modelState.ToList())
                {
                    KeyValuePair<string, ModelState> _modelState = item;

                    foreach (var _errore in _modelState.Value.Errors.ToList())
                    {
                        esito.AppendFormat("{0}<br/>", _errore.ErrorMessage);
                    }
                }

                if (string.IsNullOrWhiteSpace(esito.ToString()))
                    esito.Append("Si e verificato un errore!");

                return esito.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal string ErrorsToString(IEnumerable<string> modelState)
        {
            try
            {
                StringBuilder esito = new StringBuilder();

                foreach (var item in modelState.ToList())
                {
                    esito.AppendFormat("{0}<br/>", item);
                }

                if (string.IsNullOrWhiteSpace(esito.ToString()))
                    esito.Append("Si e verificato un errore!");

                return esito.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal T GeModelWithPaging<T, T1>(int? page, IEnumerable<T1> _query, object model, int pageSize) where T : new()
        {
            int take = MVCPaging.StartRow(page.GetValueOrDefault()) + 1;

            int skip = (MVCPaging.EndtRow() + take) - 1;

            int totalRecords = _query.Count();

            T returnModel = new T();

            void getProperty(string prop, object value)
            {
                try
                {
                    Reflection.GetProperty<T>(prop)?.SetValue(returnModel, value);
                }
                catch
                {
                }
            };

            List<T1> t = new List<T1>();

            foreach (var item in _query.Take(skip).Skip(take - 1))
            {
                t.Add(item);
            }

            getProperty("TotalRecords", totalRecords);
            getProperty("Result", t);
            getProperty("CurrentPage", page.GetValueOrDefault());
            getProperty("PageSize", pageSize);
            getProperty("Filtri", model);

            return returnModel;
        }

        internal T GeModelWithPaging<T, T1>(int? page, IEnumerable<T1> _query, object model, int totalRecords, int pageSize) where T : new()
        {
            T returnModel = new T();

            void getProperty(string prop, object value)
            {
                try
                {
                    Reflection.GetProperty<T>(prop)?.SetValue(returnModel, value);
                }
                catch
                {
                }
            };

            getProperty("TotalRecords", totalRecords);
            getProperty("Result", _query);
            getProperty("CurrentPage", page.GetValueOrDefault());
            getProperty("PageSize", pageSize);
            getProperty("Filtri", model);

            return returnModel;
        }

        #endregion

        public int ParseInt(object val)
        {
            int.TryParse(val?.ToString(), out int x);

            return x;
        }

        public Task<bool> SendMailAsync(SimpleMailMessage message)
        {
            var _username = User != null && User.Identity.IsAuthenticated ? User.Identity.Name : "";
            var _ruolo = User != null && User.Identity.IsAuthenticated ? GetUserRole() : "";
            try
            {
                var _config = ConfigurationProvider.Instance.GetConfiguration().MailSetting;

                var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(_config.SmtpServer, _config.SmtpServerPort, _config.SmtpServerUseSSL);

                // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (_config.SmtpServerAutentication)
                    client.Authenticate(_config.SmtpServerUsername, _config.SmtpServerPassword);

                var msg = new MimeMessage();

                if (message.FromEmail != null)
                {
                    msg.From.Add(new MailboxAddress(message.FromName, message.FromEmail));
                }
                else
                {
                    msg.From.Add(new MailboxAddress(_config.FromName, _config.FromEmail));
                }

                msg.To.Add(new MailboxAddress(message.ToName, message.ToEmail));

                if (message.CcEmail != null)
                {
                    msg.Cc.Add(new MailboxAddress(message.CcName, message.CcEmail));
                }

                if (message.BccEmail != null)
                {
                    msg.Bcc.Add(new MailboxAddress(message.BccName, message.BccEmail));
                }

                BodyBuilder bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = message.Body;

                //if (_config.AllegatoStream != null)
                //{
                //    try
                //    {
                //        bodyBuilder.Attachments.Add(Path.GetFileName(_config.Allegato), _config.AllegatoStream);

                //    }
                //    catch
                //    {
                //    }
                //}

                msg.Subject = message.Subject;
                msg.Body = bodyBuilder.ToMessageBody();

                client.Send(msg);
                client.Disconnect(true);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    try
                    {
                        UnitOfWork _unitofwork = new UnitOfWork();

                        _unitofwork.LogsRepository.Insert(new DOM.Entitys.Logs
                        {
                            Data = DateTime.Now,
                            Ruolo = _ruolo,
                            Username = _username,
                            Model = typeof(Exception).AssemblyQualifiedName,
                            ViewDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(ex),
                            Message = ex.Message,
                            Action = "SendMailAsync"
                        });
                        _unitofwork.Save();
                    }
                    catch
                    {
                    }
                });
                return Task.FromResult(false);
            }
        }

        public void TrimAll(object model)
        {
            foreach (var item in model.GetType().GetProperties())
            {
                try
                {
                    if (item.PropertyType == typeof(string))
                    {
                        var _val = item.GetValue(model);
                        if (_val != null)
                        {
                            item.SetValue(model, HttpUtility.UrlDecode(_val.ToString()).TrimAll());
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public string GetIP(HttpContextBase request)
        {
            try
            {
                if (request?.Request.Headers["CF-CONNECTING-IP"] != null) 
                    return request.Request.Headers["CF-CONNECTING-IP"].ToString();

                if (request?.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    string ipAddress = request.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        string[] addresses = ipAddress.Split(',');
                        if (addresses.Length != 0)
                        {
                            return addresses[0];
                        }
                    }
                }

                return request?.Request.UserHostAddress;
            }
            catch
            {
                return "";
            }
        }

    }
}
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sediin.MVC.HtmlHelpers
{
    public class Mailhelper
    {
        public bool SmtpClientUseDefaultCredentials { get; set; }
        public string SSLSmtpServer { get; set; }
        public int SSLSmtpServerPort { get; set; }
        public string SSLSmtpServerUsername { get; set; }
        public string SSLSmtpServerPassword { get; set; }
        public string SSLSmtpServerSenderEmail { get; set; }
        public bool SmtpServerAutentication { get; set; }
        public bool SmtpServerUseSSL { get; set; }
        public string MailTo { get; set; }
        public string[] MailTos { get; set; }
        public string MailCC { get; set; }
        public string[] MailCCs { get; set; }
        public string MailBCC { get; set; }
        public string Messaggio { get; set; }
        public string Oggetto { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpServerEmailFrom { get; set; }
        public bool Inviata { get; internal set; }
        public string Esito { get; internal set; }
        public string SmtpServerUsername { get; set; }
        public string SmtpServerPassword { get; set; }
        public string SmtpServerSenderEmail { get; set; }
        public string Allegato { get; set; }
        public Stream AllegatoStream { get; set; }
    }

    public class BaseController : Controller
    {
        public int? ToInt(object val)
        {
            try
            {
                if (val != null)
                    return Convert.ToInt32(val);

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DateTime? ToDate(object val)
        {
            try
            {
                if (val != null)
                    return Convert.ToDateTime(val);

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public decimal? ToDecimal(object val)
        {
            try
            {
                if (val != null)
                    return Math.Abs(Convert.ToDecimal(val));

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal ActionResult ToAlert(string message, string header = null)
        {
            var _s = "";

            if (!string.IsNullOrWhiteSpace(header))
            {
                _s += "<div class=\"lead\">" + header + "</div><hr /><div class=\"clearfix\"></div>";
            }

            _s += "<div class=\"alert alert-warning\" role=\"alert\" style=\"margin-top:25px\">";
            _s += message;
            _s += "</div>";
            return this.Content(_s);
        }

        internal static string ModelStateErrorToString(ModelStateDictionary modelState)
        {
            try
            {
                List<string> _errors = new List<string>();

                StringBuilder sb = new StringBuilder();

                foreach (var item in modelState.Values)
                {
                    if (item.Errors.Count() > 0)
                    {
                        foreach (var error in item.Errors)
                        {
                            if (_errors.FirstOrDefault(e => e.ToLower() == error.ErrorMessage.ToLower()) == null)
                                _errors.Add(error.ErrorMessage);
                        }
                    }
                }

                if (_errors.Count() > 0)
                {
                    sb.Append("<ul>");

                    foreach (var item in _errors)
                    {
                        sb.Append("<li>" + item + "</li>");
                    }

                    sb.Append("</ul>");
                }

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        public Task<bool> SendMailSSL(Mailhelper _config)
        {
            try
            {
                var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(_config.SSLSmtpServer, _config.SSLSmtpServerPort, _config.SmtpServerUseSSL);

                // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                //if (needsUserAndPwd)
                //{
                // Note: only needed if the SMTP server requires authentication
                if (_config.SmtpServerAutentication)
                    client.Authenticate(_config.SSLSmtpServerUsername, _config.SSLSmtpServerPassword);
                //}

                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_config.SSLSmtpServerSenderEmail));

                if (_config.MailTo != null)
                {
                    msg.To.Add(new MailboxAddress(_config.MailTo?.ToLower()));
                }

                if (_config.MailTos != null)
                {
                    List<MailboxAddress> _m = new List<MailboxAddress>();

                    foreach (var item in _config.MailTos)
                    {
                        _m.Add(new MailboxAddress(item?.ToLower()));
                    }

                    msg.To.AddRange(_m);
                }


                if (_config.MailCCs != null)
                {
                    List<MailboxAddress> _m = new List<MailboxAddress>();

                    foreach (var item in _config.MailCCs)
                    {
                        _m.Add(new MailboxAddress(item?.ToLower()));
                    }

                    msg.Cc.AddRange(_m);
                }

                if (!string.IsNullOrWhiteSpace(_config.MailCC))
                    msg.Cc.Add(new MailboxAddress(_config.MailCC.ToLower()));


                if (!string.IsNullOrWhiteSpace(_config.MailBCC))
                    msg.Cc.Add(new MailboxAddress(_config.MailBCC));

                BodyBuilder bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = _config.Messaggio;

                if (_config.AllegatoStream != null)
                {
                    try
                    {
                        bodyBuilder.Attachments.Add(Path.GetFileName(_config.Allegato), _config.AllegatoStream);

                    }
                    catch
                    {
                    }
                }

                msg.Subject = _config.Oggetto;
                msg.Body = bodyBuilder.ToMessageBody();

                client.Send(msg);
                client.Disconnect(true);

                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> SendMail(Mailhelper _config)
        {
            try
            {
                SmtpClient smtp;

                smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                if (_config.SmtpClientUseDefaultCredentials)
                {
                    smtp.UseDefaultCredentials = true;
                    //smtp.Credentials = new NetworkCredential();// (_config.SmtpServerUsername, _config.SmtpServerPassword);
                }
                else
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_config.SmtpServerUsername, _config.SmtpServerPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                }

                MailMessage message = new MailMessage();

                message.To.Add(new MailAddress(_config.MailTo));

                if (!string.IsNullOrWhiteSpace(_config.MailBCC))
                {
                    foreach (var item in _config.MailBCC.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                            message.Bcc.Add(new MailAddress(item.Trim()));
                    }
                }

                if (!string.IsNullOrWhiteSpace(_config.MailCC))
                {
                    foreach (var item in _config.MailCC.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                            message.CC.Add(new MailAddress(item));
                    }
                }

                message.From = new MailAddress(_config.SmtpServerSenderEmail);

                if (_config.Allegato != null)
                {
                    message.Attachments.Add(new Attachment(_config.AllegatoStream, Path.GetFileName(_config.Allegato), MimeMapping.GetMimeMapping(_config.Allegato)));
                }

                message.Subject = _config.Oggetto;
                message.Body = _config.Messaggio;

                message.IsBodyHtml = true;

                //smtp.EnableSsl = true;
                smtp.Send(message);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                // WriteLog(ex);
                _config.Inviata = false;
                _config.Esito = ex.Message;
                return Task.FromResult(false);
            }
            finally
            {
                //NinjectControllerFactory factory = new NinjectControllerFactory();
                //var repositoryRichiestaRegistroInvioMail = factory.GetController<Dom.Abstract.IRichiestaRegistroInvioMail>();
                //repositoryRichiestaRegistroInvioMail.Insert(model);
            }
        }

        public Task<bool> SendMailTLS(Mailhelper _config)
        {
            try
            {
                SmtpClient smtp;

                smtp = new SmtpClient();

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_config.SmtpServerUsername, _config.SmtpServerPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage message = new MailMessage();

                message.To.Add(new MailAddress(_config.MailTo));

                if (!string.IsNullOrWhiteSpace(_config.MailBCC))
                {
                    foreach (var item in _config.MailBCC.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                            message.Bcc.Add(new MailAddress(item.Trim()));
                    }
                }

                if (!string.IsNullOrWhiteSpace(_config.MailCC))
                {
                    foreach (var item in _config.MailCC.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                            message.CC.Add(new MailAddress(item));
                    }
                }

                message.From = new MailAddress(_config.SmtpServerSenderEmail);

                if (_config.Allegato != null)
                {
                    message.Attachments.Add(new Attachment(_config.AllegatoStream, Path.GetFileName(_config.Allegato), MimeMapping.GetMimeMapping(_config.Allegato)));
                }

                message.Subject = _config.Oggetto;
                message.Body = _config.Messaggio;

                message.IsBodyHtml = true;

                smtp.EnableSsl = false;
                smtp.Send(message);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                // WriteLog(ex);
                _config.Inviata = false;
                _config.Esito = ex.Message;
                return Task.FromResult(false);
            }
            finally
            {
                //NinjectControllerFactory factory = new NinjectControllerFactory();
                //var repositoryRichiestaRegistroInvioMail = factory.GetController<Dom.Abstract.IRichiestaRegistroInvioMail>();
                //repositoryRichiestaRegistroInvioMail.Insert(model);
            }
        }

        public ActionResult AjaxView(string view, object model = null)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(view, model);
            }
            return View(view, model);
        }

        public void ParameterToModel<T>(T model)
        {
            PropertyInfo[] p = typeof(T).GetProperties();

            foreach (var item in p)
            {
                var _v = Request.Params[item.Name];

                try
                {
                    if (_v != null)
                    {
                        Type t = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;

                        object safeValue = (_v == null) ? null : Convert.ChangeType(_v, t);

                        item.SetValue(model, safeValue, null);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// Key , Value
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public SelectList CreateSelectList(IEnumerable<object> enumerable)
        {
            try
            {
                var _l = enumerable.ToList();
                _l.Insert(0, new KeyValuePair<int?, string>(null, "[Seleziona un valore]"));
                return new SelectList(_l, "key", "value");
            }
            catch
            {
                var _l = new string[] { "[Seleziona un valore]" };
                return new SelectList(_l);
            }
        }

    }
}

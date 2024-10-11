using EBLIG.Utils;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Models;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class ConfigurationController : BaseController
    {
        public ActionResult RagioneSociale()
        {
            var model = Reflection.CreateModel<RagioneSocialeConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().RagioneSociale);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult RagioneSociale(RagioneSocialeConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.RagioneSociale = Reflection.CreateModel<RagioneSociale>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Ragione sociale aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Sepa()
        {
            var model = Reflection.CreateModel<SepaConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().Sepa);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Sepa(SepaConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.Sepa = Reflection.CreateModel<Sepa>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Sepa aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }


        public ActionResult Ftp()
        {
            var model = Reflection.CreateModel<FTPConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().Ftp);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Ftp(FTPConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.Ftp = Reflection.CreateModel<FTP>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Ftp aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Email()
        {
            var model = Reflection.CreateModel<MailSettingConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().MailSetting);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Email(MailSettingConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.MailSetting = Reflection.CreateModel<MailSetting>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Email settings aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult EmailTest()
        {
            return AjaxView(model:new TestMailSettingConfigModel { Oggetto="test", Messaggio="test" });
        }

        [HttpPost]
        public ActionResult EmailTest(TestMailSettingConfigModel model)
        {
            var send = SendMailAsync(new SimpleMailMessage
            {
                Body = model.Messaggio,
                Subject = model.Oggetto,
                ToEmail = model.EmailTo
            });

            if (send.Result == true)
            {
                return JsonResultTrue("Mail inviata a: " + model.EmailTo);
            }

            return JsonResultFalse("Si e verificato un errore nel invio mail, controlla la pagina Logs");
        }
    }
}
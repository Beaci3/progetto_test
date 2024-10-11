using EBLIG.DOM.Importer;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using EBLIG.WebUI.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [EBLIGAuthorize]
    [AuthorizeAdmin]
    public class ImporterController : BaseController
    {
        // GET: Backend/Importer
        public ActionResult Index()
        {
            return AjaxView();
        }

        public ActionResult Import()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var tipoimport = Request.Form["tipoimport"];
                    var anno = Request.Form["anno"];

                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];

                    byte[] inputBuffer = new byte[file.InputStream.Length];
                    file.InputStream.Read(inputBuffer, 0, inputBuffer.Length);

                    var _ms = new MemoryStream(inputBuffer);

                    ImportProvider p = new ImportProvider
                    {
                        Username = User.Identity.Name,
                        Ruolo = GetUserRole(),
                        FileStream = _ms,
                        TipoImport = tipoimport,
                        Anno = ParseInt(anno),
                    };

                    p.OnReport += P_OnReport;
                    p.OnErrorFile += P_OnErrorFile;
                    Task.Run(() => p.ProcessImport());

                    // Returns message that successfully uploaded  
                    return JsonResultTrue("File caricato");
                }
                catch (Exception ex)
                {
                    return JsonResultFalse(ex.Message);
                }
            }
            else
            {
                return Json("Nessun file selezionato");
            }
        }

        private void P_OnErrorFile(string base64, string tipo, string username, string ruolo)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onReportImportError(base64, username);

            Task.Run(() =>
            {
                try
                {
                    unitOfWork.LogsRepository.Insert(new DOM.Entitys.Logs
                    {
                        Data = DateTime.Now,
                        Ruolo = ruolo,
                        Username = username,
                        Model = null,
                        ViewDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64)).Replace(Environment.NewLine, "<br/>"),
                        Message = "Errore Import " + tipo,
                        Action = "Import"
                    });
                    unitOfWork.Save();
                }
                catch
                {
                }
            });
        }

        private void P_OnReport(string processoId, string username, string tipoImport, int index, int totale, string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<EBLIGHub>();
            context.Clients.All.onReportImportStatus(processoId, username, tipoImport, index, totale, message);
        }
    }
}
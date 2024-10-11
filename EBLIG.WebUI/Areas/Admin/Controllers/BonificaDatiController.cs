using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Data;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class BonificaDatiController : BaseController
    {
        // GET: Admin/BonificaDati
        public ActionResult Index()
        {
            return AjaxView();
        }

        public ActionResult AnagraficaDipendenti()
        {
            try
            {
                StringBuilder sblog = new StringBuilder();
                int _tot = 0;
                var sql = "select CodiceFiscale from( select * from(   ";
                sql += "SELECT distinct  ";
                sql += "[CodiceFiscale], COUNT([CodiceFiscale]) as tot  ";
                sql += "FROM [Eblig].[dbo].[Dipendente] ";
                sql += "group by [CodiceFiscale]) as tb ";
                sql += "where tot > 1 ) as tb";

                EBLIGDbContext c = new EBLIGDbContext();
                var l = c.Database.SqlQuery<string>(sql);

                foreach (var item in l)
                {
                    var _c = unitOfWork.DipendenteRepository.Get(x => x.CodiceFiscale == item).ToArray();

                    int[] _a = _c.Select(xx => xx.DipendenteId).ToArray();

                    var _pra = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => _a.Contains((int)x.DipendenteId));

                    var _idexclude = new List<int>
                    {
                        _a.FirstOrDefault()
                    };

                    if (_pra?.Count() > 0)
                    {
                        _idexclude = new List<int>();
                        _idexclude.AddRange(_pra.Select(x => (int)x.DipendenteId).ToList());
                    }

                    for (int i = 0; i < _c.Count(); i++)
                    {
                        try
                        {
                            var _id = _c[i].DipendenteId;

                            if (_idexclude.Contains(_id))
                            {
                                continue;
                            }
                            var codfis = _c[i].CodiceFiscale;

                            var _aziendeassociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == _id);
                            if (_aziendeassociate.Count() > 0)
                            {
                                foreach (var az in _aziendeassociate)
                                {
                                    unitOfWork.DipendenteAziendaRepository.Delete(az.DipendenteAziendaId);
                                }
                            }
                            _tot++;
                            unitOfWork.DipendenteRepository.Delete(_id);
                            unitOfWork.Save(false);
                            //sblog.Append("Codice fiscale " + codfis + " bonificato<br/>");

                        }
                        catch (Exception ex)
                        {
                            sblog.AppendLine(ex.ToString());
                        }
                    }
                }

                if (sblog.Length > 0 || _tot > 0)
                {
                    sblog.Insert(0, "<strong>Anagrafica dipendenti bonificata: </strong><br/><br/>" + "Dipendenti bonificati: " + _tot + "<br/>");
                    return JsonResultTrue(sblog.ToString());
                }

                return JsonResultTrue("Nessun dipendenti da bonificare");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

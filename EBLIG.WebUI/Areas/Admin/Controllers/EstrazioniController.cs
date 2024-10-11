using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EBLIG.DOM.DAL;
using EBLIG.DOM;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Areas.Backend.Controllers;
using EBLIG.WebUI.Filters;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class EstrazioniController : BaseController
    {
        // GET: Admin/Estrazioni
        public ActionResult Index()
        {
            return AjaxView();
        }


        public ActionResult RichiesteConsulente()
        {
            try
            {
                var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.ConsulenteCS != null);


                var _consulente = _p.Select(c => new
                {
                    c.ConsulenteCS.ConsulenteCSId,
                    c.ConsulenteCS.RagioneSociale,
                    c.ConsulenteCS.CodiceFiscalePIva
                }).Distinct();


                var _statoPratica = unitOfWork.StatoPraticaRepository.Get();

                List<EstrazioneRichiesteModel> _l = new List<EstrazioneRichiesteModel>();

                int getStato(string text)
                {
                    var _e = Enum.TryParse(text, true, out EbligEnums.StatoPratica en);
                    return (int)en;
                };

                foreach (var consulente in _consulente)
                {
                    EstrazioneRichiesteModel _es = new EstrazioneRichiesteModel
                    {
                        RagioneSociale = consulente.RagioneSociale,
                        CodiceFiscalePIva = consulente.CodiceFiscalePIva,
                        Bozza = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("Bozza")).Count(),
                        Inviata = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("Inviata")).Count(),
                        InviataRevisionata = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("InviataRevisionata")).Count(),
                        Annullata = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("Annullata")).Count(),
                        Revisione = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("Revisione")).Count(),
                        Confermata = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId && x.StatoPraticaId == getStato("Confermata")).Count(),
                        Totale = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId).Count(),
                    };


                    _l.Add(_es);

                }

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "RichiesteConsulente");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult RichiesteConsulenteBonificaAnagraficaAzienda()
        {
            try
            {

                var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.ConsulenteCS != null);

                var _consulente = _p.Select(c => new
                {
                    c.ConsulenteCS.ConsulenteCSId,
                    c.ConsulenteCS.RagioneSociale,
                    c.ConsulenteCS.CodiceFiscalePIva,
                    c.ConsulenteCS.Email,
                    c.ConsulenteCS.Telefono
                }).Distinct();


                List<EstrazioneRichiesteBonificaAnagrafica> _l = new List<EstrazioneRichiesteBonificaAnagrafica>();

                foreach (var consulente in _consulente)
                {
                    List<string> _matricole = new List<string>();

                    EstrazioneRichiesteBonificaAnagrafica _es = new EstrazioneRichiesteBonificaAnagrafica
                    {
                        RagioneSociale = consulente.RagioneSociale,
                        CodiceFiscalePIva = consulente.CodiceFiscalePIva,
                        Email = consulente.Email,
                        Telefono = consulente.Telefono
                    };

                    var _aziende = _p.Where(x => x.ConsulenteCSId == consulente.ConsulenteCSId).Select(x => x.Azienda);

                    foreach (var item in _aziende)
                    {
                        var _validate = IsValidModel(new object[] { item });

                        if (_validate.Count() > 0)
                        {
                            if (_matricole.FirstOrDefault(x => x == item.MatricolaInps) == null)
                            {
                                _matricole.Add(item.MatricolaInps);
                            }
                        }
                    }

                    if (_matricole.Count() > 0)
                    {
                        _es.DaBonificare = string.Join("; ", _matricole);
                        _l.Add(_es);
                    }
                }

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "RichiesteConsulenteBonificaAnagraficaAzienda");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult RichiesteSportelloBonificaAnagraficaDipendente()
        {
            try
            {

                var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.Sportello != null);

                var _consulente = _p.Select(c => new
                {
                    c.Sportello.SportelloId,
                    c.Sportello.RagioneSociale,
                    c.Sportello.CodiceFiscalePIva,
                    c.Sportello.Email,
                    c.Sportello.Telefono
                }).Distinct();


                List<EstrazioneRichiesteBonificaAnagrafica> _l = new List<EstrazioneRichiesteBonificaAnagrafica>();

                foreach (var consulente in _consulente)
                {
                    List<string> _matricole = new List<string>();

                    EstrazioneRichiesteBonificaAnagrafica _es = new EstrazioneRichiesteBonificaAnagrafica
                    {
                        RagioneSociale = consulente.RagioneSociale,
                        CodiceFiscalePIva = consulente.CodiceFiscalePIva,
                        Email = consulente.Email,
                        Telefono = consulente.Telefono
                    };

                    var _aziende = _p.Where(x => x.SportelloId == consulente.SportelloId).Select(x => x.Dipendente);

                    foreach (var item in _aziende)
                    {
                        var _validate = IsValidModel(new object[] { item });

                        if (_validate.Count() > 0)
                        {
                            if (_matricole.FirstOrDefault(x => x == item.CodiceFiscale) == null)
                            {
                                _matricole.Add(item.CodiceFiscale);
                            }
                        }
                    }

                    if (_matricole.Count() > 0)
                    {
                        _es.DaBonificare = string.Join("; ", _matricole);
                        _l.Add(_es);
                    }
                }


                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "RichiesteSportelloBonificaAnagraficaDipendente");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult ImportiRichiestaPerTipoRichiesta()
        {
            try
            {
                PraticheAziendaController c = new PraticheAziendaController();
                var _l = from d in c.GetImportiRichiestaPerTipoRichiesta()
                         select new EstrazioneVisualizzaBudgetViewModel
                         {
                             BudgetDisposizione = d.TipoRichiesta.BudgetDisponibile,
                             Descrizione = d.TipoRichiesta.Descrizione,
                             Anno = d.TipoRichiesta.Anno,
                             TipoRichiesta = d.TipoRichiesta.IsTipoRichiestaDipendente.GetValueOrDefault() ? "Dipendenti" : "Azienda",
                             ImportoRichiestoInviato = d.ImportoRichiesto,
                             ImportoRichiestoBozza = d.ImportoRichiestoBozza,
                             ImportoRichiestoConfermato = d.ImportoRichiestoConfermato,
                             ImportoRichiestoRevisione = d.ImportoRichiestoRevisione,
                             TotaleRichiesto = d.ImportoRichiesto + d.ImportoRichiestoBozza + d.ImportoRichiestoConfermato + d.ImportoRichiestoRevisione
                         };

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l.OrderBy(x => x.TipoRichiesta).ThenByDescending(x => x.Anno).ThenByDescending(x => x.Descrizione), "ImportiPerTipoRichiesta");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult RichiesteSportelloSindacale()
        {
            try
            {
                var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.SportelloId != null);


                var _consulente = _p.Select(c => new
                {
                    c.Sportello.SportelloId,
                    c.Sportello.RagioneSociale,
                    c.Sportello.CodiceFiscalePIva
                }).Distinct();


                var _statoPratica = unitOfWork.StatoPraticaRepository.Get();

                List<EstrazioneRichiesteModel> _l = new List<EstrazioneRichiesteModel>();

                int getStato(string text)
                {
                    var _e = Enum.TryParse(text, true, out EbligEnums.StatoPratica en);
                    return (int)en;
                };

                foreach (var consulente in _consulente)
                {
                    EstrazioneRichiesteModel _es = new EstrazioneRichiesteModel
                    {
                        RagioneSociale = consulente.RagioneSociale,
                        CodiceFiscalePIva = consulente.CodiceFiscalePIva,
                        Bozza = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("Bozza")).Count(),
                        Inviata = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("Inviata")).Count(),
                        InviataRevisionata = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("InviataRevisionata")).Count(),
                        Annullata = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("Annullata")).Count(),
                        Revisione = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("Revisione")).Count(),
                        Confermata = _p.Where(x => x.SportelloId == consulente.SportelloId && x.StatoPraticaId == getStato("Confermata")).Count(),
                        Totale = _p.Where(x => x.SportelloId == consulente.SportelloId).Count(),
                    };
                    _l.Add(_es);
                }

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "RichiesteSportelloSindacale");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult TotaleAccessiData()
        {
            try
            {
                StatisticheController c = new StatisticheController();
                var _l = c.GetUtentiGiorno();

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "TotaleAccessiData");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult TotaleRichiesteDataInvio()
        {
            try
            {
                StatisticheController c = new StatisticheController();
                var _l = c.GetPraticheDataInvio();

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "TotaleRichiesteDataInvio");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult TotaleRichieste()
        {
            try
            {
                StatisticheController c = new StatisticheController();
                var _l = c.GetPraticheAzienda();

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "TotaleRichieste");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult TotaleRichiestePerStato()
        {
            try
            {
                StatisticheController c = new StatisticheController();
                var _l = c.GetPratichePerStato();

                if (_l.Count() > 0)
                {
                    ExcelHelper _excel = new ExcelHelper();
                    return _excel.CreateExcel(_l, "TotaleRichiestePerStato");
                }

                else
                {
                    return Content("Nessuna estrazione da salvare");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
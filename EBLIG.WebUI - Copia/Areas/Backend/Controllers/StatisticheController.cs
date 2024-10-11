using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using EBLIG.DOM.Data;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    public class StatisticheController : BaseController
    {
        private System.Random random;
        //private System.Reflection.PropertyInfo[] a;

        public StatisticheController()
        {
            random = new Random();

            //a = typeof(System.Drawing.Color).GetProperties();

        }

        public string Color(int? index = null)
        {
            string[] colors = null;
            try
            {
                colors = new string[]
                {
                    "rgb(255,0,0)",
                    "rgb(255,140,0)",
                    "rgb(189,183,107)",
                    "rgb(0,100,0)",
                    "rgb(128,128,0)",
                    "rgb(0,128,128)",
                    "rgb(30,144,255)",
                    "rgb(123,104,238)",
                    "rgb(128,0,128)",
                    //"rgb(255,20,147)",
                    "rgb(255,0,255)",
                    "rgb(128,0,0)",
                    "rgb(255,228,225)",
                };
                return colors[index.GetValueOrDefault()];
            }
            catch
            {
                return GetColor(colors);
            }
        }

        private string GetColor(string[] colors)
        {
            var _color = System.Drawing.Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

            //var _color = (System.Drawing.Color)a[r.Next(a.Length)].GetValue(null, null);

            var _rgb = "rgb(" + _color.R + "," + _color.G + "," + _color.B + ")";

            if (colors.FirstOrDefault(c => c == _rgb) != null)
            {
                return GetColor(colors);
            }

            return _rgb;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> PraticheDataInvio()
        {
            CultureInfo _culture = new CultureInfo("it-IT");

            string getDate(string x)
            {
                var _datax = Convert.ToDateTime(x);

                var _mese = _culture.DateTimeFormat.GetMonthName(_datax.Month);

                return $"{_datax.Day.ToString().PadLeft(2, '0')} {_mese} {_datax.Year}";

            };

            var _n = unitOfWork.PraticheRegionaliImpreseRepository.Get();

            var _d = _n.Where(x => x.DataInvio != null).Select(x => x.DataInvio.Value.ToShortDateString()).Distinct();

            var stat = (from x in _d
                        select new Statistiche
                        {
                            Descrizione = getDate(x),
                            Totale = _n.Where(c => c.DataInvio != null && c.DataInvio.Value.ToShortDateString() == x).Count()
                        });

            var _data = (stat != null) ? stat.ToArray().OrderByDescending(x => x.Totale).Take(10).ToArray() : new Statistiche[] { };
            var model = GetChartModel(_data, "Top 10 giorni invio richieste");

            return await Task.FromResult(PartialView("PieChart", model));
        }

        public async Task<PartialViewResult> PraticheAzienda()
        {
            var r = unitOfWork.PraticheRegionaliImpreseRepository.Get().Select(x => new
            {
                x.TipoRichiestaId,
                x.TipoRichiesta.Descrizione,
                x.TipoRichiesta.Anno,
                x.TipoRichiesta.IsTipoRichiestaDipendente
            });

            var _tiprich = r.Select(x => new { x.TipoRichiestaId, x.Descrizione, x.Anno, x.IsTipoRichiestaDipendente }).Distinct();

            var stat = _tiprich.Select(x => new Statistiche
            {
                Descrizione = x.Descrizione + " " + x.Anno + (x.IsTipoRichiestaDipendente == true ? " (Dipendente)" : " (Azienda)"),
                Totale = r.Where(c => c.TipoRichiestaId == x.TipoRichiestaId).Count()
            });

            var _data = (stat != null) ? stat.ToArray() : new Statistiche[] { };
            var model = GetChartModel(_data, "Per tipo richiesta");

            return await Task.FromResult(PartialView("PieChart", model));
        }

        public async Task<PartialViewResult> PratichePerStato()
        {
            var r = unitOfWork.PraticheRegionaliImpreseRepository.Get().Select(x => new
            {
                x.StatoPraticaId,
                x.StatoPratica.Descrizione
            });

            var _stato = r.Select(x => x.StatoPraticaId).Distinct();

            var stat = _stato.Select(x => new Statistiche
            {
                Descrizione = r.FirstOrDefault(c => c.StatoPraticaId == x).Descrizione,
                Totale = r.Where(c => c.StatoPraticaId == x).Count()
            });

            var _data = (stat != null) ? stat.ToArray() : new Statistiche[] { };
            var model = GetChartModel(_data, "Per stato richiesta");

            return await Task.FromResult(PartialView("PieChart", model));
        }

        public async Task<PartialViewResult> UtentiGiorno()
        {
            CultureInfo _culture = new CultureInfo("it-IT");

            string getDate(string x)
            {
                var _datax = Convert.ToDateTime(x);

                var _mese = _culture.DateTimeFormat.GetMonthName(_datax.Month);

                return $"{_datax.Day.ToString().PadLeft(2, '0')} {_mese} {_datax.Year}";

            };

            var _n = unitOfWork.NavigatioHistoryRepository.Get();

            var _d = _n.Select(x => x.Data.Value.ToShortDateString()).Distinct();

            var stat = (from x in _d
                        select new Statistiche
                        {
                            Descrizione = getDate(x),
                            Totale = _n.Where(c => c.Data.Value.ToShortDateString() == x).Select(d => d.Username).Distinct().Count()
                        });

            var _data = (stat != null) ? stat.ToArray().OrderByDescending(x => x.Totale).Take(10).ToArray() : new Statistiche[] { };
            var model = GetChartModel(_data, "Top 10 giorni accessi");

            return await Task.FromResult(PartialView("PieChart", model));
        }


        private ChartModel GetChartModel(Statistiche[] data, string titolo)
        {
            var result = new List<ChartDataModel>();

            for (int i = 0; i < data.Count(); i++)
            {
                result.Add(new ChartDataModel
                {
                    Data = data[i].Totale.GetValueOrDefault(),
                    Label = data[i].Totale.GetValueOrDefault() + " - " + data[i].Descrizione,// + " - " + ,
                    Color = Color(i)
                });
            }

            var model = new ChartModel
            {
                ChartData = result,
                ChartTitle = titolo
            };
            return model;
        }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EBLIG.DOM;
using EBLIG.DOM.DAL;
using Sediin.MVC.HtmlHelpers;

namespace Estrazione
{
    internal class Program
    {
        public class AziendePraticheDiretteEstrazione
        {
            public string Diretta { get; set; }
            public string RagioneSociale { get; set; }
            public string CodiceFiscale { get; set; }
            public string PIva { get; set; }
            public string MatricolaInps { get; set; }

            public string Indirizzo { get; set; }
            public string Pec { get; set; }
            public string Email { get; set; }

            public string RagioneSocialeSportello { get; set; }
            public string ProvinciaSportello { get; set; }


            public string RagioneSocialeConsulente{ get; set; }
            public string ProvinciaConsulente { get; set; }

            public string TipologiaIntervento { get; set; }
            public string StatoRichiesta { get; set; }
            public int? AnnoCompetenza { get; set; }
            public DateTime? DataRichiesta { get; set; }

            public decimal? ImportoContributo { get; set; }
            public decimal? AliquoteIRPEF { get; set; }
            public decimal? ImportoIRPEF { get; set; }
            public decimal? ImportoContributoNetto { get; set; }




        }

        public class ConsulenteEstrazione
        {
            public string RagioneSociale { get; set; }
            public string CodiceFiscalePIva { get; set; }

            public int? Bozza { get; set; }

            public int? Inviata { get; set; }

            public int? Annullata { get; set; }

            public int? Revisione { get; set; }

            public int? InviataRevisionata { get; set; }

            public int? Confermata { get; set; }

            public int? Totale { get; set; }

        }

        public class AnagraficaBonificaEstrazione
        {
            public string RagioneSociale { get; set; }
            public string CodiceFiscalePIva { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }

            public int TotaleAziende { get; set; }

            public string DaBonificare { get; set; }

        }
        static void Main(string[] args)
        {
            EstrazioneAziendaDiretto();







            //EstrazioneDipendentiBonifica();

            //Console.WriteLine("");

            //EstrazioneAziendeBonifica();

            //Console.WriteLine("");

            //EstrazioneConsulenteRichieste();

            //Console.WriteLine("");

            //EstrazioneSportelloSindacaleRichieste();

            //Console.ReadLine();

        }

        private static void EstrazioneAziendaDiretto()
        {
            try
            {
            Console.WriteLine("Attendere, creazione estrazione in corso per Pratiche Dirette aziende...");

            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Estrazione\\AziendePraticheDirette";

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var _filename = Path.Combine(_path, "AziendePraticheDirette_" + Guid.NewGuid().ToString() + ".xlsx");

            UnitOfWork unitOfWork = new UnitOfWork();

            var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => !(bool)x.TipoRichiesta.IsTipoRichiestaDipendente);

            List<AziendePraticheDiretteEstrazione> _l = new List<AziendePraticheDiretteEstrazione>();

            foreach (var item in _p)
            {
                int? _anno = null;

                if (item.DataConferma.HasValue)
                {
                    _anno = item.DataConferma.Value.Year;
                }

                AziendePraticheDiretteEstrazione _a = new AziendePraticheDiretteEstrazione
                {
                    AliquoteIRPEF = item.AliquoteIRPEF,
                    AnnoCompetenza = _anno,
                    CodiceFiscale = item.Azienda.CodiceFiscale,
                    DataRichiesta = item.DataInvio,
                    Diretta = item.Sportello != null ? "Indiretta" : "Diretta",
                    Email = item.Azienda.Email,
                    ImportoContributo = item.ImportoContributo,
                    ImportoContributoNetto = item.ImportoContributoNetto,
                    ImportoIRPEF = item.ImportoIRPEF,
                    Indirizzo = ($"{item.Azienda.Indirizzo} - {item.Azienda.Comune?.DENCOM?.Trim()} {item.Azienda.Localita?.CAP} {item.Azienda.Provincia?.DENPRO?.Trim()}").Trim(),
                    MatricolaInps = item.Azienda.MatricolaInps,
                    Pec = item.Azienda.Pec,
                    PIva = item.Azienda.PartitaIva,
                    RagioneSociale = item.Azienda.RagioneSociale,
                    TipologiaIntervento = $"{item.TipoRichiesta?.Descrizione} - {item.TipoRichiesta?.Anno}",
                    StatoRichiesta = item.StatoPratica.Descrizione,
                    RagioneSocialeSportello = item.Sportello?.RagioneSociale,
                    ProvinciaSportello = item.Sportello?.Provincia?.DENPRO,
                    RagioneSocialeConsulente = item.ConsulenteCS?.RagioneSociale,
                    ProvinciaConsulente = item.ConsulenteCS?.Provincia?.DENPRO
                };

                _l.Add(_a);
            }

            if (_l.Count() > 0)
            {
                var _b = CreateExcelBase64(_l);
                File.WriteAllBytes(_filename, Convert.FromBase64String(_b));
                Console.WriteLine("Estrazione creata in " + _filename);
            }

            else
            {
                Console.WriteLine("Nessuna estrazione salvata");
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }

        private static void EstrazioneDipendentiBonifica()
        {
            Console.WriteLine("Attendere, creazione estrazione in corso per Sportello Sindacale dipoendenti da bonificare...");

            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Estrazione\\SportelloSindacale";

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var _filename = Path.Combine(_path, "DipendentiDaBonificare_" + Guid.NewGuid().ToString() + ".xlsx");

            UnitOfWork unitOfWork = new UnitOfWork();

            var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.Sportello != null);

            var _consulente = _p.Select(c => new
            {
                c.Sportello.SportelloId,
                c.Sportello.RagioneSociale,
                c.Sportello.CodiceFiscalePIva,
                c.Sportello.Email,
                c.Sportello.Telefono
            }).Distinct();


            List<AnagraficaBonificaEstrazione> _l = new List<AnagraficaBonificaEstrazione>();

            foreach (var consulente in _consulente)
            {
                List<string> _matricole = new List<string>();

                AnagraficaBonificaEstrazione _es = new AnagraficaBonificaEstrazione
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
                var _b = CreateExcelBase64(_l);
                File.WriteAllBytes(_filename, Convert.FromBase64String(_b));
                Console.WriteLine("Estrazione creata in " + _filename);
            }

            else
            {
                Console.WriteLine("Nessuna estrazione salvata");
            }
        }

        private static void EstrazioneAziendeBonifica()
        {
            Console.WriteLine("Attendere, creazione estrazione in corso per Consulenti aziende da bonificare...");

            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Estrazione\\Consulente";

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var _filename = Path.Combine(_path, "AziendeDaBonificare_" + Guid.NewGuid().ToString() + ".xlsx");

            UnitOfWork unitOfWork = new UnitOfWork();

            var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.ConsulenteCS != null);

            var _consulente = _p.Select(c => new
            {
                c.ConsulenteCS.ConsulenteCSId,
                c.ConsulenteCS.RagioneSociale,
                c.ConsulenteCS.CodiceFiscalePIva,
                c.ConsulenteCS.Email,
                c.ConsulenteCS.Telefono
            }).Distinct();


            List<AnagraficaBonificaEstrazione> _l = new List<AnagraficaBonificaEstrazione>();

            foreach (var consulente in _consulente)
            {
                List<string> _matricole = new List<string>();

                AnagraficaBonificaEstrazione _es = new AnagraficaBonificaEstrazione
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
                var _b = CreateExcelBase64(_l);
                File.WriteAllBytes(_filename, Convert.FromBase64String(_b));
                Console.WriteLine("Estrazione creata in " + _filename);
            }

            else
            {
                Console.WriteLine("Nessuna estrazione salvata");
            }
        }

        private static void EstrazioneConsulenteRichieste()
        {
            Console.WriteLine("Attendere, creazione estrazione in corso per Richieste per Consulenti...");

            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Estrazione\\Consulente";

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            var _filename = Path.Combine(_path, "Estrazione_" + Guid.NewGuid().ToString() + ".xlsx");

            UnitOfWork unitOfWork = new UnitOfWork();

            var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.ConsulenteCS != null);


            var _consulente = _p.Select(c => new
            {
                c.ConsulenteCS.ConsulenteCSId,
                c.ConsulenteCS.RagioneSociale,
                c.ConsulenteCS.CodiceFiscalePIva
            }).Distinct();


            var _statoPratica = unitOfWork.StatoPraticaRepository.Get();

            List<ConsulenteEstrazione> _l = new List<ConsulenteEstrazione>();

            int getStato(string text)
            {
                var _e = Enum.TryParse(text, true, out EbligEnums.StatoPratica en);
                return (int)en;
            };

            foreach (var consulente in _consulente)
            {
                ConsulenteEstrazione _es = new ConsulenteEstrazione
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
                var _b = CreateExcelBase64(_l);
                File.WriteAllBytes(_filename, Convert.FromBase64String(_b));
                Console.WriteLine("Estrazione creata in " + _filename);
            }

            else
            {
                Console.WriteLine("Nessuna estrazione salvata");
            }

        }

        private static void EstrazioneSportelloSindacaleRichieste()
        {
            Console.WriteLine("Attendere, creazione estrazione in corso per Richieste per Sportello sindacale...");

            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Estrazione\\SportelloSindacale";

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            var _filename = Path.Combine(_path, "Estrazione_" + Guid.NewGuid().ToString() + ".xlsx");

            UnitOfWork unitOfWork = new UnitOfWork();

            var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.SportelloId != null);


            var _consulente = _p.Select(c => new
            {
                c.Sportello.SportelloId,
                c.Sportello.RagioneSociale,
                c.Sportello.CodiceFiscalePIva
            }).Distinct();


            var _statoPratica = unitOfWork.StatoPraticaRepository.Get();

            List<ConsulenteEstrazione> _l = new List<ConsulenteEstrazione>();

            int getStato(string text)
            {
                var _e = Enum.TryParse(text, true, out EbligEnums.StatoPratica en);
                return (int)en;
            };

            foreach (var consulente in _consulente)
            {
                ConsulenteEstrazione _es = new ConsulenteEstrazione
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
                var _b = CreateExcelBase64(_l);
                File.WriteAllBytes(_filename, Convert.FromBase64String(_b));
                Console.WriteLine("Estrazione creata in " + _filename);
            }

            else
            {
                Console.WriteLine("Nessuna estrazione salvata");
            }

        }

        static string CreateExcelBase64<T>(IEnumerable<T> model)
        {
            try
            {

                if (model == null || model?.Count() == 0)
                    return "Nessun record trovato";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(Reflection.ListToDataTable<T>(model.ToList()));
                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static List<string> IsValidModel(object[] value)
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

    }
}

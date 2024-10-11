using DocumentFormat.OpenXml.EMMA;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Models;
using EBLIG.Utils;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Areas.Backend.Controllers
{
    [EBLIGAuthorize]
    [AuthorizeAdmin]
    public class UniemensController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(UniemensRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.UniemensRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<UniemensRicercaViewModel, Uniemens>(page, _query, model, totalRows, model.PageSize);
            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(UniemensRicercaModel model)
        {
            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(RicercaDataTable(model), "Contributi");
        }

        private SqlLam<Uniemens> RicercaFilter2(UniemensRicercaModel model)
        {
            var f = new SqlLam<Uniemens>();

            if (model.UniemensRicercaModel_AziendaId.HasValue)
            {
                f.And(x => x.AziendaId == model.UniemensRicercaModel_AziendaId);
            }

            if (model.UniemensRicercaModel_Anno.HasValue)
            {
                f.And(x => x.Anno == model.UniemensRicercaModel_Anno);
            }

            if (model.UniemensRicercaModel_UniemensId.HasValue)
            {
                f.And(x => x.UniemensId == model.UniemensRicercaModel_UniemensId);
            }

            return f;
        }

        private Expression<Func<Uniemens, bool>> RicercaFilter(UniemensRicercaModel model)
        {
            return x => ((model.UniemensRicercaModel_AziendaId != null ? x.AziendaId == model.UniemensRicercaModel_AziendaId : true)
            && (model.UniemensRicercaModel_Anno != null ? x.Anno == model.UniemensRicercaModel_Anno : true)
            && (model.UniemensRicercaModel_UniemensId != null ? x.UniemensId == model.UniemensRicercaModel_UniemensId : true));
        }

        DataTable RicercaDataTable(UniemensRicercaModel model, string mese = null)
        {
            DataTable table = new DataTable("Grid");

            var rowIndex = 0;

            foreach (var item in unitOfWork.UniemensRepository.Get(RicercaFilter(model)))
            {
                var uniemensModel = JsonConvert.DeserializeObject<UniemensModel>(item.UniemensBson);

                //crea DataTable
                if (rowIndex == 0)
                {
                    var uniemens = uniemensModel.mensilita.FirstOrDefault();

                    table.Columns.Add("Ragione sociale");
                    table.Columns.Add("Matricola Inps");
                    table.Columns.Add("Dipendente");
                    table.Columns.Add("Codice Fiscale");
                    table.Columns.Add("Mese", typeof(int));
                    table.Columns.Add("Anno", typeof(int));
                    table.Columns.Add("Versamenti", typeof(decimal));
                    table.Columns.Add("Movimenti", typeof(decimal));
                    table.Columns.Add("Imponibile", typeof(decimal));

                    foreach (var colonne in ConfigurationProvider.Instance.GetConfiguration().Uniemens.Colonna)
                    {
                        table.Columns.Add(colonne, typeof(decimal));
                    }

                    rowIndex++;
                }

                var _dovutiDipendente = uniemensModel.mensilita.Select(x => x.dovuti).ToArray();

                foreach (var mensilita in uniemensModel.mensilita.Where(x => mese != null ? x.mese == mese : true))
                {
                    for (int i = 0; i < mensilita.dovuti.Count(); i++)
                    {
                        var _dov = mensilita.dovuti[i];
                        DataRow row = table.NewRow();
                        row["Ragione sociale"] = $"{item.Azienda?.RagioneSociale}";
                        row["Matricola Inps"] = $"{item.Azienda?.MatricolaInps}";
                        row["Dipendente"] = $"{_dov.cognome} {_dov.nome}";
                        row["Codice Fiscale"] = _dov.codice_fiscale;

                        row["Mese"] = mensilita.mese;
                        row["Anno"] = item.Anno;
                        row["Versamenti"] = mensilita.totali.entrate;
                        row["Movimenti"] = mensilita.totali.movimenti;
                        row["Imponibile"] = _dov.imponibile;

                        foreach (var colonne1 in ConfigurationProvider.Instance.GetConfiguration().Uniemens.Colonna)
                        {
                            decimal _importo = 0;

                            if (_dov.quote?.FirstOrDefault(x => x.quota == colonne1) != null)
                            {
                                decimal.TryParse(_dov.quote.FirstOrDefault(x => x.quota == colonne1)?.importo?.ToString(), out decimal import);

                                _importo = import;
                            }

                            row[colonne1] = _importo;
                        }

                        table.Rows.Add(row);
                    }
                }
            }

            return table;
        }

        #endregion

        public ActionResult Dipendenti(int uniemensId, string mese)
        {
            UniemensMensilitaViewModel model = new UniemensMensilitaViewModel();
            model.Uniemens = unitOfWork.UniemensRepository.Get(x => x.UniemensId == uniemensId).FirstOrDefault();
            model.Mensilita = JsonConvert.DeserializeObject<UniemensModel>(model.Uniemens?.UniemensBson)?.mensilita?.Where(x => x.mese == mese);
            return AjaxView("Dipendenti", model);
        }

        public ActionResult DipendentiExcel(int uniemensId, string mese)
        {
            UniemensRicercaModel model = new UniemensRicercaModel();
            model.UniemensRicercaModel_UniemensId = uniemensId;

            var _uniemens = unitOfWork.UniemensRepository.Get(RicercaFilter(model)).FirstOrDefault();
            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(RicercaDataTable(model, mese), $"Contributi_Dipendenti_{_uniemens.Azienda.MatricolaInps}_{mese}_{_uniemens.Anno}");
        }
    }
}
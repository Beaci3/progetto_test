using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Admin.Models;
using EBLIG.WebUI.Areas.Backend.Controllers;
using EBLIG.WebUI.Areas.Backend.Models;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Filters;
using LambdaSqlBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class CopertureController : BaseController
    {
        // GET: Backend/Copertura
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(CoperturaModel model, int? page)
        {
            var _query = unitOfWork.CoperturaRepository.Get(RicercaFilter(model)).OrderBy(m => m.AziendaId);

            var _result = GeModelWithPaging<CoperturaModelRicercaViewModel, Copertura>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Copertura, bool>> RicercaFilter(CoperturaModel model)
        {
            return x => model.AziendaId != null ? x.AziendaId == model.AziendaId : true;
        }

        public ActionResult RicercaExcel(CoperturaRicercaModel model)
        {
            var _query = from a in unitOfWork.CoperturaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             //a.AziendaId,
                             a.Azienda?.RagioneSociale,
                             a.Azienda?.MatricolaInps,
                             a.Azienda?.CodiceFiscale,
                             a.Azienda?.Email,
                             Coperto = a.Coperto ? "Coperto" : "Non coperto",
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Coperture");
        }

        private Expression<Func<Copertura, bool>> RicercaFilter2(CoperturaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(CoperturaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _Copertura = unitOfWork.CoperturaRepository.Get(m => m.AziendaId == model.AziendaId).ToList();
                if (_Copertura.Count > 0)
                {
                    throw new Exception("Copertura già presente.");
                }

                //check se Azienda esiste
                var _Az = unitOfWork.AziendaRepository.Get(m => m.AziendaId == model.AziendaId).ToList();
                if (_Az.Count == 0)
                {
                    throw new Exception("Matricola errata.");
                }

                //se Copertura non esiste creo
                var _nuovoCopertura = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Copertura>(model);
                _nuovoCopertura.AziendaId = (int)model.AziendaId;

                if (model.Coperto != null)
                {
                    _nuovoCopertura.Coperto = (bool)model.Coperto;
                }
                else
                {
                    _nuovoCopertura.Coperto = false;
                }
                unitOfWork.CoperturaRepository.Insert(_nuovoCopertura);
                unitOfWork.Save(false);
                return JsonResultTrue("Record Copertura inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Copertura = unitOfWork.CoperturaRepository.Get(m => m.CoperturaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<CoperturaModel>(_Copertura);
            _l.Ragionesociale = _Copertura.Azienda.RagioneSociale;
            _l.MatricolaInps = _Copertura.Azienda.MatricolaInps;

            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(CoperturaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.CoperturaRepository.Get(m => m.CoperturaId == model.CoperturaId).FirstOrDefault();

                //check se Copertura esiste
                var _Copertura = unitOfWork.CoperturaRepository.Get(m => m.AziendaId == model.AziendaId).ToList();
                if (_Copertura.Count > 0 && model.AziendaId != _l.AziendaId)
                {
                    throw new Exception("Copertura già presente.");
                }

                //check se Azienda esiste
                var _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaId).FirstOrDefault();
                if (_azienda == null)
                {
                    throw new Exception("Azienda non trovata");
                }

                //se non esiste allora modifico
                _l.AziendaId = (int)model.AziendaId;
                if (model.Coperto != null)
                {
                    _l.Coperto = (bool)model.Coperto;
                }
                else
                {
                    _l.Coperto = false;
                }
                unitOfWork.CoperturaRepository.Update(_l);
                unitOfWork.Save(false);
                return JsonResultTrue("Copertura aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}
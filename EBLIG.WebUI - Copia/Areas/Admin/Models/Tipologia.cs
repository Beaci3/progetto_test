using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class TipologiaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Tipologia> Result { get; set; }

        public TipologiaModel Filtri { get; set; }
    }

    public class TipologiaModel
    {
        public int TipologiaId { get; set; }
        public string Descrizione { get; set; }
        public bool? Partesociale { get; set; }
    }

    public class InsTipologia
    {
        [Required]
        [DisplayName("Codice Tipologia")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Tipologia valido")]
        public int TipologiaId { get; set; }
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? Partesociale { get; set; }
    }


    public class TipologiaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
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

    public class TipologiaSearchModel
    {
        public string Descrizione { get; set; }
    }

    public class TipologiaModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int TipologiaId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? Partesociale { get; set; }
    }

    public class InsTipologia
    {
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
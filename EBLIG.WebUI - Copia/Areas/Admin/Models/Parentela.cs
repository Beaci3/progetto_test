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
    public class ParentelaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Parentela> Result { get; set; }

        public ParentelaModel Filtri { get; set; }
    }

    public class ParentelaModel
    {
        public int ParentelaId { get; set; }
        public string Descrizione { get; set; }
        public string Note { get; set; }
    }

    public class InsParentela
    {
        public int ParentelaId { get; set; }
        [Required]
        [DisplayName("Codice Parentela")]
        public string Descrizione { get; set; }
        [Required]
        [DisplayName("Descrizione")]
        public string Note { get; set; }
    }

    public class ParentelaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }


}
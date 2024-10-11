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
    public class CoperturaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Copertura> Result { get; set; }

        public CoperturaModel Filtri { get; set; }
    }
    public class CoperturaSearchModel
    {
        public int? AziendaId { get; set; }
        public string Ragionesociale { get; set; }
    }


    public class CoperturaModel
    {
        [Required(ErrorMessage ="Modello dati errati!")]
        public int CoperturaId { get; set; }
        [Required]
        public int? AziendaId { get; set; }
        public string Ragionesociale { get; set; }
        public string MatricolaInps { get; set; }
        public bool? Coperto { get; set; }
    }

    public class InsCopertura
    {
        [Required]
        [DisplayName("Codice Copertura")]
        public int? AziendaId { get; set; }
        [Required]
        [DisplayName("Matricola")]
        public bool? Coperto { get; set; }
    }


    public class CoperturaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
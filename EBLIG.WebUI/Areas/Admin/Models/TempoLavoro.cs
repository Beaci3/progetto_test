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
    public class TempoLavoroModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<TempoLavoro> Result { get; set; }

        public TempoLavoroModel Filtri { get; set; }
    }
    public class TempoLavoroSearchModel
    {
        public string Descrizione { get; set; }
    }

    public class TempoLavoroModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int TempoLavoroId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? TempoPieno { get; set; }
    }

    public class InsTempoLavoro
    {
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? TempoPieno { get; set; }
    }

    public class TempoLavoroRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
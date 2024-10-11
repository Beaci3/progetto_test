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
    public class TipoContrattoModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<TipoContratto> Result { get; set; }

        public TipoContrattoModel Filtri { get; set; }
    }
    public class TipoContrattoSearchModel
    {
        public string Descrizione { get; set; }
    }

    public class TipoContrattoModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int TipoContrattoId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
    }

    public class InsTipoContratto
    {
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
    }


    public class TipoContrattoRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
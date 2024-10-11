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
    public class TipoImpiegoModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<TipoImpiego> Result { get; set; }

        public TipoImpiegoModel Filtri { get; set; }
    }

    public class TipoImpiegoSearchModel
    {
        public string Descrizione { get; set; }
    }


    public class TipoImpiegoModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int TipoImpiegoId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
    }

    public class InsTipoImpiego
    {
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
    }


    public class TipoImpiegoRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
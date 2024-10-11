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

    public class TipoImpiegoModel
    {
        public int TipoImpiegoId { get; set; }
        public string Descrizione { get; set; }
    }

    public class InsTipoImpiego
    {
        [Required]
        [DisplayName("Codice Tipo Impiego")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Tipo Impiego valido")]
        public int TipoImpiegoId { get; set; }
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
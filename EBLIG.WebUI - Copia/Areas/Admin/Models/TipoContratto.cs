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

    public class TipoContrattoModel
    {
        public int TipoContrattoId { get; set; }
        public string Descrizione { get; set; }
    }

    public class InsTipoContratto
    {
        [Required]
        [DisplayName("Codice Tipo Contratto")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Tipo Contratto valido")]
        public int TipoContrattoId { get; set; }
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
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
    public class StatoPraticaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<StatoPratica> Result { get; set; }

        public StatoPraticaModel Filtri { get; set; }
    }

    public class StatoPraticaSearchModel
    {
        public string Descrizione { get; set; }
    }


    public class StatoPraticaModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int StatoPraticaId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? ReadOnly { get; set; }
        [Required(ErrorMessage = "Sequenza Obbligatoria!")]
        [DisplayName("Ordine")]
        public int Ordine { get; set; }
    }

    public class InsStatoPratica
    {
        [Required]
        [DisplayName("Codice Stato Pratica")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Stato Pratica valido")]
        public int StatoPraticaId { get; set; }
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public bool? ReadOnly { get; set; }
        [Required(ErrorMessage = "Sequenza Obbligatoria!")]
        [DisplayName("Ordine")]
        public int Ordine { get; set; }
    }


    public class StatoPraticaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
}
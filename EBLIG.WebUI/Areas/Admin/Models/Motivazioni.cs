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
    public class MotivazioniModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Motivazioni> Result { get; set; }

        public MotivazioniModel Filtri { get; set; }
    }

    public class MotivazioniModel
    {
        public int MotivazioniId { get; set; }
        public int StatoPraticaId { get; set; }
        public string Motivazione { get; set; }
        public string Note { get; set; }
        public int? MotivazioniRicercaModel_StatoPraticaId { get; set; }
    }

    public class InsMotivazioni
    {
        public int MotivazioniId { get; set; }
        [Required]
        [DisplayName("Codice Motivazione")]
        public int StatoPraticaId { get; set; }
        [Required]
        [DisplayName("Codice Stato Pratica")]
        public string Motivazione { get; set; }
        [Required]
        [DisplayName("Motivazione")]
        public string Note { get; set; }
        public IEnumerable<StatoPratica> StatoPratica { get; set; }
    }

    public class MotivazioniRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Motivazione";
    }


}
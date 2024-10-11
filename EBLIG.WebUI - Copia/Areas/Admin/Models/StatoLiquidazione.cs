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
    public class StatoLiquidazioneModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<StatoLiquidazione> Result { get; set; }

        public StatoLiquidazioneModel Filtri { get; set; }
    }

    public class StatoLiquidazioneModel
    {
        public int StatoLiquidazioneId { get; set; }
        public string Descrizione { get; set; }
        public int? Ordine { get; set; }
    }

    public class InsStatoLiquidazione
    {
        [Required]
        [DisplayName("Codice Stato Liquidazione")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Stato Liquidazione valido")]
        public int StatoLiquidazioneId { get; set; }
        [Required]
        [DisplayName("Descrizione")]
        public string Descrizione { get; set; }
        public int Ordine { get; set; }
    }

    public class StatoLiquidazioneRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }


}
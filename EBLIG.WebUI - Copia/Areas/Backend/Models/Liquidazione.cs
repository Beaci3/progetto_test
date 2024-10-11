using EBLIG.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{

    public class LiquidazioneDaLiquidareRicercaModel
    {
        public int? PraticheAziendaRicercaModel_TipoRichiestaId { get; set; }

        public int? PraticheAziendaRicercaModel_DipendenteId { get; set; }

        public string PraticheAziendaRicercaModel_NominativoDipendente { get; set; }

        public int? PraticheAziendaRicercaModel_AziendaId { get; set; }

        public string PraticheAziendaRicercaModel_RagioneSociale { get; set; }

        public List<TipoRichiesta> TipoRichiesta { get; set; }

        public int? ListaPraticheLiquidazione { get; set; }
    }

    public class LiquidazioneDaLiquidareRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public decimal? ImportoDaLiquidare { get; set; }
        public decimal? ImportoListaPraticheLiquidazione { get; set; }

        public IEnumerable<PraticheRegionaliImprese> Result { get; set; }

        public LiquidazioneDaLiquidareRicercaModel Filtri { get; set; }

        public List<int> ListaPraticheLiquidazione { get; set; }
    }

    public class LiquidazioneRicercaModel
    {

        public int? LiquidazioneRicercaModel_StatoLiquidazioneId { get; set; }

        public List<StatoLiquidazione> StatoLiquidazione { get; set; }
    }

    public class LiquidazioneRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public decimal? ImportoDaLiquidare { get; set; }
        public decimal? ImportoLiquidato { get; set; }
        public decimal? ImportoInLiquidazione { get; set; }
        public decimal? ImportoAnnullato { get; set; }

        public IEnumerable<Liquidazione> Result { get; set; }

        public LiquidazioneRicercaModel Filtri { get; set; }
    }

    public class LiquidazioneViewModel
    {
        public Liquidazione Liquidazione { get; set; }

        public List<StatoLiquidazione> StatoLiquidazione { get; set; }
    }

    public class LiquidazioneAnnullaViewModel
    {
        [Required]
        public int LiquidazioneId { get; set; }

        public string Allegato { get; set; }

        public string Note { get; set; }
    }

    public class LiquidazioneLavoraViewModel
    {
        [Required]
        public int LiquidazioneId { get; set; }

        public string Allegato { get; set; }

        public string Note { get; set; }
    }
}
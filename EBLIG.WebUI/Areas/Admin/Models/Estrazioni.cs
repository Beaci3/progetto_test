using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EBLIG.DOM.Entitys;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class EstrazioneRichiesteModel
    {
        public string RagioneSociale { get; set; }

        public string CodiceFiscalePIva { get; set; }

        public int? Bozza { get; set; }

        public int? Inviata { get; set; }

        public int? Annullata { get; set; }

        public int? Revisione { get; set; }

        public int? InviataRevisionata { get; set; }

        public int? Confermata { get; set; }

        public int? Totale { get; set; }

    }

    public class EstrazioneRichiesteBonificaAnagrafica
    {
        public string RagioneSociale { get; set; }

        public string CodiceFiscalePIva { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public int TotaleAziende { get; set; }

        public string DaBonificare { get; set; }

    }

    public class EstrazioneVisualizzaBudgetViewModel
    {
        public string TipoRichiesta { get; set; }

        public string Descrizione { get; set; }

        public int? Anno { get; set; }

        public decimal? ImportoRichiestoBozza { get; set; }
        public decimal? ImportoRichiestoInviato { get; set; }
        public decimal? ImportoRichiestoRevisione { get; set; }
        public decimal? ImportoRichiestoConfermato { get; set; }
        public decimal? TotaleRichiesto{ get; set; }

        public decimal? BudgetDisposizione { get; set; }
    }

}
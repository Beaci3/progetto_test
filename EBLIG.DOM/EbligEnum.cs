using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM
{
    public static class EbligEnums
    {
        public static AzioniPratica EnumAzioni(this AzioniPratica azione)
        {
            return azione;
        }

        public static StatoPratica EnumStatoPratica(this StatoPratica stato)
        {
            return stato;
        }

        public static StatoPratica EnumStatoLiquidazione(this StatoPratica stato)
        {
            return stato;
        }

        public enum StatoLiqidazione
        {
            InLiquidazione = 1,
            Liquidata = 2,
            Annullata = 3
        }

        public enum StatoPratica
        {
            Bozza = 1,
            Inviata = 2,
            Annullata = 3,
            Revisione = 4,
            InviataRevisionata = 5,
            Confermata = 6
        }

        public enum AzioniPratica
        {
            Bozza,
            Invia,
            Conferma,
            Revisione,
            Annulla,
            InviaRevisionata,
            BozzaRevisionata
        }
    }
}

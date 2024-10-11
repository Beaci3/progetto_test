using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("Liquidazione")]
    public class Liquidazione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LiquidazioneId { get; set; }

        public DateTime DataCreazione { get; set; }

        public DateTime? DataLavorazione { get; set; }

        public int StatoLiquidazioneId { get; set; }
        [ForeignKey("StatoLiquidazioneId")]
        public virtual StatoLiquidazione StatoLiquidazione { get; set; }

        [ForeignKey("LiquidazioneId")]
        public virtual ICollection<LiquidazionePraticheRegionaliMailInviatiEsito> MailInviate { get; set; }

        public virtual ICollection<LiquidazionePraticheRegionali> LiquidazionePraticheRegionali { get; set; }

        [MaxLength(50)]
        public string Allegato { get; set; }

        public string Note { get; set; }
        public int? MailDaInviareTotale{ get; set; }
    }

    [Table("LiquidazionePraticheRegionali")]
    public class LiquidazionePraticheRegionali
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LiquidazionePraticheRegionaliId { get; set; }

        public int LiquidazioneId { get; set; }
        [ForeignKey("LiquidazioneId")]
        public virtual Liquidazione Liquidazione { get; set; }

        public int PraticheRegionaliImpreseId { get; set; }
        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual PraticheRegionaliImprese PraticheRegionaliImprese { get; set; }

       // public decimal? Importo { get; set; }
    }

    [Table("StatoLiquidazione")]
    public class StatoLiquidazione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatoLiquidazioneId { get; set; }

        [MaxLength(75)]
        public string Descrizione { get; set; }

        public int? Ordine { get; set; }
    }

    [Table("LiquidazionePraticheRegionaliMailInviatiEsito")]
    public class LiquidazionePraticheRegionaliMailInviatiEsito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LiquidazionePraticheRegionaliMailInviatiEsitoId { get; set; }

        [MaxLength(75)]
        public string Email { get; set; }

        [MaxLength(1000)]
        public string Esito { get; set; }

        public int LiquidazioneId { get; set; }
        [ForeignKey("LiquidazioneId")]
        public virtual Liquidazione Liquidazione { get; set; }

        public bool Inviata { get; set; }
    }


}

using EBLIG.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("Dipendente")]
    public class Dipendente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DipendenteId { get; set; }

        [Required]
        [DisplayName("Codice Fiscale")]
        [MaxLength(16)]
        public string CodiceFiscale { get; set; }

        [Required]
        [DisplayName("Nome")]
        [MaxLength(175)]
        public string Nome { get; set; }

        [Required]
        [DisplayName("Cognome")]
        [MaxLength(175)]
        public string Cognome { get; set; }

        [Required]
        [DisplayName("Data nascita")]
        public DateTime? Datanascita { get; set; }

        [Required]
        [DisplayName("Regione nascita")]
        public int? RegioneNascitaId { get; set; }

        [Required]
        [DisplayName(displayName: "Provincia nascita")]
        public int? ProvinciaNascitaId { get; set; }

        [Required]
        [DisplayName("Comune nascita")]
        public int? ComuneNascitaId { get; set; }

        [ForeignKey("RegioneNascitaId")]
        public virtual Regioni RegioneNascita { get; set; }

        [ForeignKey("ProvinciaNascitaId")]
        public virtual Province ProvinciaNascita { get; set; }

        [ForeignKey("ComuneNascitaId")]
        public virtual Comuni ComuneNascita { get; set; }

        [Required]
        [MaxLength(175)]
        public string Indirizzo { get; set; }

        [Required]
        [DisplayName("Regione")]
        public int? RegioneId { get; set; }

        [Required]
        [DisplayName(displayName: "Provincia")]
        public int? ProvinciaId { get; set; }

        [Required]
        [DisplayName("Comune")]
        public int? ComuneId { get; set; }

        [Required]
        [DisplayName("Localita")]
        public int? LocalitaId { get; set; }

        [ForeignKey("RegioneId")]
        public virtual Regioni Regione { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Province Provincia { get; set; }

        [ForeignKey("ComuneId")]
        public virtual Comuni Comune { get; set; }

        [ForeignKey("LocalitaId")]
        public virtual Localita Localita { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(175)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Cellulare { get; set; }

        [ForeignKey("DipendenteId")]
        public virtual ICollection<DipendenteAzienda> Aziende { get; set; }

        //[Required]
        [MaxLength(50)]
        public string Iban { get; set; }

        public int? SportelloId { get; set; }

        [ForeignKey("SportelloId")]
        public virtual Sportello Sportello { get; set; }
    }

    /// <summary>
    /// relazione dipendente e azienda
    /// </summary>
    [Table("DipendenteAzienda")]
    public class DipendenteAzienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DipendenteAziendaId { get; set; }

        public int AziendaId { get; set; }

        public virtual Azienda Azienda { get; set; }

        public int DipendenteId { get; set; }

        public virtual Dipendente Dipendente { get; set; }

        [MaxLength(175)]
        [DisplayName("CCNL / CNEL")]
        public string CCNLCNEL { get; set; }

        [Required]
        [DisplayName("Data assunzione")]
        public DateTime? DataAssunzione { get; set; }

        [DisplayName("Data cessione")]
        public DateTime? DataCessazione { get; set; }

        [Required]
        [DisplayName("Tipo impiego")]
        public int? TipoImpiegoId { get; set; }
        public virtual TipoImpiego TipoImpiego { get; set; }

        [Required]
        [DisplayName("Tipo contratto")]
        public int? TipoContrattoId { get; set; }
        public virtual TipoContratto TipoContratto { get; set; }

        [Required]
        [DisplayName("Tempo lavoro")]
        public int? TempoLavoroId { get; set; }
        public virtual TempoLavoro TempoLavoro { get; set; }

        [DisplayName("Documento Identità")]
        public string DocumentoIdentita { get; set; }

        [DisplayName("Documento altro")]
        public string DocumentoAltro { get; set; }
    }

    [Table("TempoLavoro")]
    public class TempoLavoro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TempoLavoroId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; }

        public bool? TempoPieno { get; set; }
    }

    [Table("TipoContratto")]
    public class TipoContratto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoContrattoId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; }
    }

    [Table("TipoImpiego")]
    public class TipoImpiego
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoImpiegoId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; }
    }
}

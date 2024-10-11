using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EBLIG.Utils;

namespace EBLIG.DOM.Entitys
{
    [Table("Azienda")]
    public class Azienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AziendaId { get; set; }

        public int? ConsulenteCSId { get; set; }

        [ForeignKey("ConsulenteCSId")]
        public virtual ConsulenteCS ConsulenteCS { get; set; }

        [Required]
        [DisplayName("Matricola Inps")]
        [RegularExpression("[0-9]{10}")]
        //[Index(IsUnique = true, Order =2)]
        [MaxLength(10)]
        public string MatricolaInps { get; set; }

        [Required]
        [DisplayName("Partita Iva")]
        [RegularExpression("[0-9]{11}", ErrorMessage = "Il campo Partita Iva è obbligatorio.\r\n")]
        [MaxLength(11, ErrorMessage = "Il campo Partita Iva non è valido")]
        public string PartitaIva { get; set; }

        [Required]
        [DisplayName("Codice Fiscale")]
        //[RegularExpression("[0-9]{11}", ErrorMessage = "Il campo Codice Fiscale è obbligatorio.\r\n")]
        [MaxLength(16, ErrorMessage = "Il campo Codice Fiscale non è valido")]
        public string CodiceFiscale { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome Titolare")]
        public string NomeTitolare { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome Titolare")]
        public string CognomeTitolare { get; set; }

        [Required]
        [MaxLength(175)]
        [DisplayName("Ragione Sociale")]
        public string RagioneSociale { get; set; }

        [MaxLength(175)]
        [DisplayName("Attività economica")]
        public string AttivitaEconomica { get; set; }

        [MaxLength(175)]
        public string Classificazione { get; set; }

        [Required]
        [MaxLength(175)]
        public string CSC { get; set; }

        //[Required]
        [MaxLength(175)]
        [DisplayName("Codice Istat")]
        public string CodiceIstat { get; set; }

        //[Required]
        [DisplayName("Tipologia")]
        public int? TipologiaId { get; set; }

        [ForeignKey("TipologiaId")]
        public virtual Tipologia Tipologia { get; set; }

        [MaxLength(1500)]
        public string Partesociale { get; set; }

        //[Required]
        [DisplayName("Data Iscrizione")]
        public DateTime DataIscrizione
        {
            get
            {
                return DateTime.Now;
            }
            set
            {

            }
        }

        [DisplayName("Data Cessazione")]
        public DateTime? DataCessazione { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Pec { get; set; }

        [MaxLength(27)]
        [Required]
        public string Iban { get; set; }

        [Required]
        [MaxLength(175)]
        public string Indirizzo { get; set; }


        private int? _RegioneId;

        [Required]
        [DisplayName("Regione")]
        public int? RegioneId
        {
            get
            {
                try
                {
                    if (_RegioneId != null)
                    {
                        return _RegioneId;
                    }

                    return ConfigurationProvider.Instance?.GetConfiguration()?.RegioneId;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                _RegioneId = value;
            }
        }

        [Required(ErrorMessage = "Provincia e un campo obbligatorio")]
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

        #region Referente

        //[Required]
        [MaxLength(175)]
        [DisplayName("Indirizzo Referente")]
        public string ReferenteIndirizzo { get; set; }

        //[Required]
        [DisplayName("Regione Referente")]
        public int? ReferenteRegioneId { get; set; }

        //[Required]
        [DisplayName(displayName: "Provincia Referente")]
        public int? ReferenteProvinciaId { get; set; }

        //[Required]
        [DisplayName("Comune Referente")]
        public int? ReferenteComuneId { get; set; }

        //[Required]
        [DisplayName("Localita Referente")]
        public int? ReferenteLocalitaId { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome Referente")]
        public string ReferenteNome { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome Referente")]
        public string ReferenteCognome { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        [Display(Name = "Email Referente")]
        public string ReferenteEmail { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        [Display(Name = "Pec Referente")]
        public string ReferentePec { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Cellulare Referente")]
        public string ReferenteCellulare { get; set; }

        [ForeignKey("ReferenteRegioneId")]
        public virtual Regioni ReferenteRegione { get; set; }

        [ForeignKey("ReferenteProvinciaId")]
        public virtual Province ReferenteProvincia { get; set; }

        [ForeignKey("ReferenteComuneId")]
        public virtual Comuni ReferenteComune { get; set; }

        [ForeignKey("ReferenteLocalitaId")]
        public virtual Localita ReferenteLocalita { get; set; }


        #endregion

        #region Rappresentante

        [Required]
        [MaxLength(175)]
        [DisplayName("Indirizzo Rappresentante")]
        public string RappresentanteIndirizzo { get; set; }

        [Required]
        [DisplayName("Regione Rappresentante")]
        public int? RappresentanteRegioneId { get; set; }

        [Required]
        [DisplayName(displayName: "Provincia Rappresentante")]
        public int? RappresentanteProvinciaId { get; set; }

        [Required]
        [DisplayName("Comune Rappresentante")]
        public int? RappresentanteComuneId { get; set; }

        [Required]
        [DisplayName("Localita Rappresentante")]
        public int? RappresentanteLocalitaId { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome Rappresentante")]
        public string RappresentanteNome { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome Rappresentante")]
        public string RappresentanteCognome { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        [Display(Name = "Email Rappresentante")]
        public string RappresentanteEmail { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        [Display(Name = "Pec Rappresentante")]
        public string RappresentantePec { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Cellulare Rappresentante")]
        public string RappresentanteCellulare { get; set; }

        [ForeignKey("RappresentanteRegioneId")]
        public virtual Regioni RappresentanteRegione { get; set; }

        [ForeignKey("RappresentanteProvinciaId")]
        public virtual Province RappresentanteProvincia { get; set; }

        [ForeignKey("RappresentanteComuneId")]
        public virtual Comuni RappresentanteComune { get; set; }

        [ForeignKey("RappresentanteLocalitaId")]
        public virtual Localita RappresentanteLocalita { get; set; }

        [ForeignKey("AziendaId")]
        public virtual ICollection<DelegheConsulenteCSAzienda> DelegheConsulenteCS { get; set; }

        [ForeignKey("AziendaId")]
        public virtual ICollection<DipendenteAzienda> Dipendenti { get; set; }

        #endregion

        [InverseProperty("Azienda")]
        public virtual ICollection<Copertura> Copertura { get; set; }
    }

    [Table("Copertura")]
    public class Copertura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CoperturaId { get; set; }

        public int AziendaId { get; set; }
        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }

        public bool Coperto { get; set; }
    }
}

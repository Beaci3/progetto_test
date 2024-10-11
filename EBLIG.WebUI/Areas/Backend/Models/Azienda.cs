using DocumentFormat.OpenXml.Wordprocessing;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.ValidationAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class AziendaRicercaModel
    {
        [MaxLength(75)]
        public string AziendaRicercaModel_RagioneSociale { get; set; }

        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Il campo Matricola Inps non è valido")]
        public string AziendaRicercaModel_MatricolaInps { get; set; }

        public int? AziendaRicercaModel_TipologiaId { get; set; }

        //[MaxLength(16)]
        //[ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale non è valido", Required = false, RequiredPivaOrCF = false)]
        [RegularExpression("[0-9]{11}", ErrorMessage = "Il campo Codice Fiscale non è valido")]
        [MaxLength(11, ErrorMessage = "Il campo Codice Fiscale non è valido")]
        public string AziendaRicercaModel_CodiceFiscale { get; set; }

        [RegularExpression("[0-9]{11}", ErrorMessage = "Il campo Partita Iva non è valido")]
        [MaxLength(11, ErrorMessage = "Il campo Partita Iva non è valido")]
        public string AziendaRicercaModel_PartitaIva { get; set; }

        public int? AziendaRicercaModel_ComuneId { get; set; }
        public string AziendaRicercaModel_Comune { get; set; }

        [MaxLength(25)]
        public string AziendaRicercaModel_CSC { get; set; }

        public IEnumerable<Tipologia> Tipologie { get; set; }
        public string AziendaRicercaModel_ConsulenteCS { get; set; }

        public string Ordine { get; set; } = "RagioneSociale asc";

        public int PageSize { get; set; } = 10;

    }

    public class AziendaRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Azienda> Result { get; set; }

        public AziendaRicercaModel Filtri { get; set; }
    }

    public class AziendaViewModel : Azienda
    {
        public bool? InformazioniPersonaliCompilati { get; set; }

        public bool? ReadOnly { get; set; }

        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = true)]
        //[RegularExpression("[a-zA-Z0-9]{11,16}", ErrorMessage = "Il campo Codice Fiscale è obbligatorio")]
        [MaxLength(16, ErrorMessage = "Il campo Codice Fiscale non è valido")]
        public new string CodiceFiscale { get; set; }

        public IEnumerable<Tipologia> Tipologie { get; set; }

        [Required]
        [DisplayName("Documento di identità del legale rappresentante")]
        public string DocumentoIdentita { get; set; }

        [Required]
        [DisplayName("Delega dell'azienda")]
        public string DelegaAzienda { get; set; }

        [Required]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public new string Iban { get; set; }

    }

    public class AziendaAssociaRicercaModel
    {
        public string AziendaAssociaRicercaModel_RagioneSocialeMatricolaInps { get; set; }

        [Required]
        [DisplayName("Ragione sociale o Matricola Inps")]
        public int AziendaAssociaRicercaModel_AziendaId { get; set; }

    }

    public class AziendaAssociaRicercaViewModel
    {
        [Required]
        [DisplayName("Azienda da associare")]
        public int AziendaId { get; set; }

        [Required]
        [DisplayName("Documento di identità del legale rappresentante")]
        public string DocumentoIdentita { get; set; }

        [Required]
        [DisplayName("Delega dell'azienda")]
        public string DelegaAzienda { get; set; }

        public IEnumerable<Azienda> Aziende { get; set; }
    }

    public class AziendaPrestazioniRegionaliViewModel
    {
        public bool IbanRequired { get; set; } = true;

        public bool? ReadOnly { get; set; }

        public string RagioneSociale { get; set; }

        public string CodiceFiscale { get; set; }

        public string MatricolaInps { get; set; }

        public string PartitaIva { get; set; }

        [Required]
        [MaxLength(30)]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public string Iban { get; set; }

        public bool AziendaCoperta { get; set; }
    }

    public class AziendaUploadAllegatoModel
    {
        [Required]
        public int DelegheConsulenteCSAziendaId { get; set; }

        [Required(ErrorMessage ="Selezionare un documento")]
        public string Allegato { get; set; }

        [Required]
        public string TipoAllegato { get; set; }

        [Required]
        public int AziendaId { get; set; }
    }

    public class DissociaCS
    {
        public int? AziendaId { get; set; }
    }

}
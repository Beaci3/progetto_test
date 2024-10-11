using DocumentFormat.OpenXml.Vml.Spreadsheet;
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
    public class DipendenteRicercaModel
    {
        [MaxLength(75)]
        public string DipendenteRicercaModel_Cognome { get; set; }

        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale non è valido", Required = false, RequiredPivaOrCF = false)]
        public string DipendenteRicercaModel_CodiceFiscale { get; set; }

        public int? DipendenteRicercaModel_ComuneId { get; set; }

        public bool? DipendenteRicercaModel_GestitoSportello { get; set; }

        public IEnumerable<Comuni> Comuni { get; set; }

        public string Ordine { get; set; } = "Nome asc, Cognome asc";

        public int PageSize { get; set; } = 10;

    }

    public class DipendenteRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Dipendente> Result { get; set; }

        public DipendenteRicercaModel Filtri { get; set; }
    }

    public class DipendenteViewModel : Dipendente
    {
        public bool? InformazioniPersonaliCompilati { get; set; }

        public bool? ReadOnly { get; set; }

        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale non è valido", Required = true, RequiredPivaOrCF = false)]
        public new string CodiceFiscale { get; set; }

        [Required]
        public new DateTime? Datanascita { get; set; }

        //[Required]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public new string Iban { get; set; }

        [Required]
        [DisplayName("Documento di identità del dipendente")]
        public string DocumentoIdentita { get; set; }

        [Required]
        [DisplayName("Delega del dipendente")]
        public string DelegaDipendente { get; set; }

    }

    public class DipendenteAssociaAziendaRicercaModel
    {
        public string DipendenteAssociaRicercaModel_NominativoCF { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public int DipendenteAssociaRicercaModel_DipendenteId { get; set; }
    }

    public class DipendenteAssociaRicercaModel
    {
        public string DipendenteAssociaRicercaModel_NominativoCF { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public int DipendenteAssociaRicercaModel_DipendenteId { get; set; }
    }

    public class DipendenteAssociaRicercaViewModel
    {
        [Required]
        [DisplayName("Azienda da associare")]
        public int DipendenteId { get; set; }

        [Required]
        [DisplayName("Documento di identità del dipendente")]
        public string DocumentoIdentita { get; set; }

        [Required]
        [DisplayName("Delega del dipendente")]
        public string DelegaDipendente { get; set; }

        public IEnumerable<Dipendente> Dipendente { get; set; }
    }

    public class DipendenteAziendaAssociaViewModel
    {
        [Required]
        [DisplayName("Azienda da associare")]
        public int AziendaId { get; set; }

        public IEnumerable<Azienda> Aziende { get; set; }

        [Required]
        [DisplayName("Documento di identità Dipendente")]
        public string DocumentoIdentita { get; set; }

        //[Required]
        //[DisplayName("Altro documento? (da definire)")]
        //public string AltroDocumento { get; set; }

        [Required]
        [MaxLength(175)]
        [DisplayName("CCNL / CNEL")]
        public string CCNLCNEL { get; set; }

        [Required]
        [DisplayName("Data assunzione")]
        public DateTime? DataAssunzione { get; set; }

        [Required]
        [DisplayName("Tipo impiego")]
        public int? TipoImpiegoId { get; set; }

        [Required]
        [DisplayName("Tipo contratto")]
        public int? TipoContrattoId { get; set; }

        [Required]
        [DisplayName("Tempo lavoro")]
        public int? TempoLavoroId { get; set; }

        public IEnumerable<TipoImpiego> TipoImpiego { get; set; }
        public IEnumerable<TipoContratto> TipoContratto { get; set; }
        public IEnumerable<TempoLavoro> TempoLavoro { get; set; }

        public string DipendenteAziendaAssociaViewModel_NominativoCF { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public int? DipendenteAziendaAssociaViewModel_DipendenteId { get; set; }

    }

    public class DipendenteAziendaCessazioneContrattoModel
    {
        [Required]
        public int DipendenteAziendaId { get; set; }

        //[Required]
        [DisplayName("Data cessazione")]
        public DateTime? DataCessione { get; set; }

        [Required]
        [DisplayName("Data assunzione")]
        [DataDal_DataAl(ErrorMessage = "La data cessazione deve essere maggiore della data assunzione", DataAlRequired = true, DataAlField = "DataCessione")]
        public DateTime? DataAssunzione{ get; set; }
    }



    public class DipendentePrestazioniRegionaliViewModel
    {
        public bool IbanRequired { get; set; } = true;

        public bool? ReadOnly { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string CodiceFiscale { get; set; }

        public DateTime? Datanascita { get; set; }

        [Required]
        [MaxLength(30)]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public string Iban { get; set; }
    }

    public class DipendenteUploadAllegatoModel
    {
        [Required]
        public int DelegheSportelloDipendenteId { get; set; }

        [Required(ErrorMessage = "Selezionare un documento")]
        public string Allegato { get; set; }

        [Required]
        public string TipoAllegato { get; set; }

        [Required]
        public int DipendenteId { get; set; }
    }

}
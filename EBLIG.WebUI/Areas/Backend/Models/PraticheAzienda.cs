using EBLIG.DOM;
using EBLIG.DOM.Entitys;
using EBLIG.Utils;
using EBLIG.WebUI.ValidationAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EBLIG.WebUI.Areas.Backend.Models.PraticheAzienda_Dipendente_CarenzaMalattia;
using static EBLIG.WebUI.Areas.Backend.Models.PraticheAzienda_IncrementoMantenimentoOccupazione;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class VisualizzaBudgetViewModel
    {
        public TipoRichiesta TipoRichiesta { get; set; }

        public decimal? ImportoRichiesto { get; set; }
        public decimal? ImportoRichiestoBozza { get; set; }
        public decimal? ImportoRichiestoRevisione { get; set; }
        public decimal? ImportoRichiestoConfermato { get; set; }
    }

    public class PraticheAziendaRicercaModel
    {

        public int? PraticheAziendaRicercaModel_TipoRichiestaId { get; set; }

        public int? PraticheAziendaRicercaModel_DipendenteId { get; set; }

        public string PraticheAziendaRicercaModel_NominativoDipendente { get; set; }

        public int? PraticheAziendaRicercaModel_AziendaId { get; set; }

        public string PraticheAziendaRicercaModel_RagioneSociale { get; set; }

        public string PraticheAziendaRicercaModel_ProtocolloId { get; set; }

        public List<TipoRichiesta> TipoRichiesta { get; set; }

        public int? PraticheAziendaRicercaModel_StatoPraticaId { get; set; }
        
        public string PraticheAziendaRicercaModel_DataInvio{ get; set; }

        public List<StatoPratica> StatoPratica { get; set; }

        public string PraticheAziendaRicercaModel_OrderBy { get; set; } = "DataInvio == null, DataInvio asc";

    }

    public class PraticheAziendaRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public decimal? ImportoLiquidato { get; set; }
        public decimal? ImportoDaLiquidare { get; set; }
        public decimal? ImportoInLiquidare { get; set; }
        public decimal? ImportoRiconoscitoNetto { get; set; }

        public IEnumerable<PraticheRegionaliImprese> Result { get; set; }

        public PraticheAziendaRicercaModel Filtri { get; set; }
    }

    public class PraticheAziendaNuovaRichiestaSportello
    {
        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public int? PraticheAziendaNuovaRichiesta_DipendenteId { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public string PraticheAziendaNuovaRichiesta_NominativoDipendente { get; set; }


    }

    public class PraticheAziendaNuovaRichiesta
    {
        [Required]
        [DisplayName("Dipendente, Nominativo o Codice Fiscale")]
        public int? PraticheAziendaNuovaRichiesta_DipendenteId { get; set; }

        [DisplayName("Nominativo")]
        public string PraticheAziendaNuovaRichiesta_NominativoDipendente { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public int? PraticheAziendaNuovaRichiesta_AziendaId { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public string PraticheAziendaNuovaRichiesta_RagioneSociale { get; set; }

        [Required]
        [DisplayName("Tipo Richiesta")]
        public int? PraticheAziendaNuovaRichiesta_TipoRichiestaId { get; set; }

        public IEnumerable<TipoRichiesta> TipoRichiesta { get; set; }

    }

    public class PraticheAziendaAllegati
    {
        public bool? ReadOnly { get; set; }
        public int? RichiestaId { get; set; }
        public List<TipoRichiestaAllegati> TipoRichiestaAllegati { get; set; }
        public List<PraticheRegionaliImpreseAllegati> RichiestaAllegati { get; set; }

        [DocumentiObblicatori(ErrorMessage = "Caricare tutti documenti obbligatori", AllegatiIdSelInput = "AllegatiIdSelInput")]
        public string AllegatiId { get; set; }

        public string AllegatiIdSelInput { get; set; }

        public PraticheAziendaAllegatiUpload[] File { get; set; }

    }

    public class PraticheAziendaAllegatiUpload
    {
        public int TipoRichiestaAllegatiId { get; set; }

        public string Base64 { get; set; }

        public string NomeFile { get; set; }

        public string Estensione { get; set; }

        public string CodTipAlldescrizione { get; set; }

        public string Completefilename { get; set; }

        public int? PraticheRegionaliImpreseAllegatiId { get; set; }
    }

    public class PraticheAziendaAzioni
    {
        public IEnumerable<Azioni> Azioni { get; set; }

        public int? RichiestaId { get; set; }

        public int? TipoRichiestaId { get; set; }

        public int? StatoId { get; set; }

        public IEnumerable<AzioniRuolo> AzioniRuolo { get; set; }

        public bool? LiquidataOinLiquidazione { get; set; }

        public bool? AbilitatoNuovaRichiesta { get; set; }
    }

    public class PraticheAziendaRevisione_Annulla
    {
        [Required]
        [DisplayName("Motivazione")]
        public int MotivazioneId { get; set; }

        [Required]
        [DisplayName("Richiesta")]
        public int RichiestaId { get; set; }

        public string Note { get; set; }

        public IEnumerable<Motivazioni> Motivazioni { get; set; }

        [Required]
        public EbligEnums.StatoPratica StatoPratica { get; set; }
    }

    public class PraticheAziendaMail
    {
        public string Nominativo { get; set; }

        public string Descrizione { get; set; }

        public string Note { get; set; }
    }


    #region tipo pratica

    public class PraticheAzienda_ImportoContributoAttribute : Attribute
    {

    }

    public class PraticheAziendaContainer
    {
        public PraticheAziendaContainer()
        {
            //StatoId = 1;//bozza
        }

        public bool? ReadOnly { get; set; }

        //[Required]
        public int RichiestaId { get; set; } = 0;

        [Required]
        public int TipoRichiestaId { get; set; }

        [Required]
        public int AziendaId { get; set; }

        public int? DipendenteId { get; set; }

        //[Required]
        public int StatoId { get; set; }

        public object DataModel { get; set; }

        public string View { get; set; }

        public string DescrizioneStato { get; set; }

        public string DescrizioneTipoRichiesta { get; set; }

        public string NoteTipoRichiesta { get; set; }

        public string Azione { get; set; }

        public string ProtocolloId { get; set; }

        public IEnumerable<PraticheRegionaliImpreseStatoPraticaStorico> StoricoStatoPratica { get; set; }

        public int? ChildClassRowCount { get; set; } = 0;

        public decimal? ImportoContributo { get; set; }

        public decimal? AliquoteIRPEF { get; set; }

        public decimal? ImportoIRPEF { get; set; }

        public decimal? ImportoContributoNetto { get; set; }

        [NotMapped]
        [MaxLength(30)]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public string Iban { get; set; }

        public bool LiquidataOinLiquidazione { get; set; }

        public PraticheRegionaliImprese PraticheRegionaliImprese { get; set; }

        public bool IbanAziendaRequired { get; set; }

        public bool IbanDipendenteRequired { get; set; }
    }

    public class PraticheAzienda_BaseClass
    {
        [NotMapped]
        public bool? ReadOnly { get; set; }

        [NotMapped]
        public int? RichiestaId { get; set; }

        [NotMapped]
        public int? AziendaId { get; set; }

        [NotMapped]
        public int? DipendenteId { get; set; }

        [NotMapped]
        public int? StatoPraticaId { get; set; }

        [NotMapped]
        public int? TipoRichiestaId { get; set; }

        [NotMapped]
        public TipoRichiesta TipoRichiesta { get; set; }

        [NotMapped]
        public int? ChildClassRowCount { get; set; } = 0;

        [NotMapped]
        public decimal? ImportoContributo { get; set; }

        [NotMapped]
        public decimal? AliquoteIRPEF { get; set; }

        [NotMapped]
        public decimal? ImportoIRPEF { get; set; }

        [NotMapped]
        public decimal? ImportoContributoNetto { get; set; }

    }

    public class PraticheAzienda_ImportoCalcolatiModel
    {
        public decimal? AliquoteIRPEF { get; set; }
        public decimal? ImportoIRPEF { get; set; }
        public decimal? ImportoContributo { get; set; }
        public decimal? ImportoContributoNetto { get; set; }
    }

    #region azienda

    public class PraticheAzienda_Maternita : PraticheAzienda_BaseClass
    {
        [Required]
        [MaxLength(175)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(175)]
        public string Cognome { get; set; }

        [DisplayName("Codice Fiscale")]
        [Required]
        [VerificaTipoRichiestaUnivocoCodiceFiscaleValidator(ErrorMessage = "Per il Codice Fiscale e già stata presentata una richiesta", NomeCampo = "CodiceFiscale", Unica = false)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = false)]
        [MaxLength(16)]
        public string CodiceFiscale { get; set; }

        [DisplayName("Data nascita")]
        [Required]
        public DateTime? DataNascita { get; set; }

        [Required]
        [MaxLength(175)]
        public string Indirizzo { get; set; }

        [Required]
        [DisplayName("Regione")]
        public int RegioneId
        {
            get
            {
                return ConfigurationProvider.Instance.GetConfiguration().RegioneId;
            }
            set
            {

            }
        }

        [Required(ErrorMessage = "Provincia e un campo obbligatorio")]
        [DisplayName("Provincia")]
        public int? ProvinciaId { get; set; }

        [Required]
        [DisplayName("Comune")]
        public int ComuneId { get; set; }

        [Required]
        [DisplayName("Localita")]
        public int LocalitaId { get; set; }

        [NotMapped]
        public Regioni Regione { get; set; }

        [NotMapped]
        public Province Provincia { get; set; }

        [NotMapped]
        public Comuni Comune { get; set; }

        [NotMapped]
        public Localita Localita { get; set; }

    }

    public class PraticheAzienda_RiduzionePremioInail : PraticheAzienda_BaseClass
    {
    }

    public class PraticheAzienda_SicurezzaSulLavoro : PraticheAzienda_BaseClass
    {
        [Required]
        [DisplayName("Motivo della richiesta")]
        public int? MotivoRichiestaId { get; set; }

        [Required]
        [CustomRangeValidator(MinValue = "1500",
            MinValueErrorMessage = "Non è possibile richiedere la prestazione per importi inferiori a € 1.500,00")]
        //MaxValue = "150000",
        //MaxValueErrorMessage = "Non è possibile richiedere la prestazione per importi superiori a € 150.000,00"
        [DisplayName("Totale delle fatture")]
        public decimal? TotaleFatture { get; set; }

        [Required]
        [RequiredFromEBLIGAdmin(ErrorMessage = "Il campo Totale delle fatture accettate e obbligatorio")]
        [CustomRangeValidator(MinValue = "1500",
            MinValueErrorMessage = "Non è possibile richiedere la prestazione per importi inferiori a € 1.500,00", ValidateOnlyForRoles = "Admin")]
        [DisplayName("Totale delle fatture accettate")]
        public decimal? TotaleFatturAccettate { get; set; } = 0;

        [Required]
        [RequiredFromEBLIGAdmin(ErrorMessage = "Il campo Importo totale rimborsato e obbligatorio")]
        [DisplayName("Importo totale rimborsato")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoTotaleRimborsato { get; set; } = 0;
    }

    public class PraticheAzienda_IncentiviImpreseCovid19 : PraticheAzienda_BaseClass
    {
        [Required]
        [DisplayName("Numero giorni di sospensione")]
        [CustomRangeValidator(MinValue = "3",
            MinValueErrorMessage = "Inserire almeno 3 giorni")]
        [PraticheAzienda_IncentiviImpreseCovid19Validation(ErrorMessage = "Importo non valido")]
        public int? NumeroGiorniSospensione { get; set; }

        [Required]
        [DisplayName("Importo totale rimborsato")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoTotaleRimborsato { get; set; }

    }

   
    public class PraticheAzienda_IncrementoMantenimentoOccupazione : PraticheAzienda_BaseClass
    {
        [Required]
        [DisplayName("Importo riconosciuto")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoRiconosciutoTotale { get; set; } = 0;

        public class Richiedente
        {
            public string ModalId { get; set; }
            public int TipoRichiestaId { get; set; }
            public int AziendaId { get; set; }
            public int? RichiestaId { get; set; }

            [Required]
            [MaxLength(175)]
            public string Nome { get; set; }

            [Required]
            [MaxLength(175)]
            public string Cognome { get; set; }

            [DisplayName("Codice Fiscale")]
            [Required]
            [VerificaTipoRichiestaUnivocoCodiceFiscaleValidator(ErrorMessage = "Per il Codice Fiscale e già stata presentata una richiesta", NomeCampo = "CodiceFiscale", Unica = false)]
            [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = false)]
            [MaxLength(16)]
            public string CodiceFiscale { get; set; }

            [DisplayName("Data nascita")]
            [Required]
            public DateTime? DataNascita { get; set; }

            [Required]
            [MaxLength(175)]
            public string Indirizzo { get; set; }

            [Required(ErrorMessage = "Regione e un campo obbligatorio")]
            [DisplayName("Regione")]
            public int? RegioneId { get; set; }
            //{
            //    get
            //    {
            //        return ConfigurationProvider.Instance.GetConfiguration().RegioneId;
            //    }
            //    set
            //    {

            //    }
            //}

            [Required(ErrorMessage = "Provincia e un campo obbligatorio")]
            [DisplayName("Provincia")]
            public int? ProvinciaId { get; set; }

            [Required]
            [DisplayName("Comune")]
            public int ComuneId { get; set; }

            [Required]
            [DisplayName("Localita")]
            public int LocalitaId { get; set; }

            [NotMapped]
            public Regioni Regione { get; set; }

            [NotMapped]
            public Province Provincia { get; set; }

            [NotMapped]
            public Comuni Comune { get; set; }

            [NotMapped]
            public Localita Localita { get; set; }

            [Required]
            [DisplayName("Ore settimanali dipendente")]
            [Range(1, 40, ErrorMessage = "Ore settimanali dipendente tra 1 e 40")]
            public int? OreSettimanaleDipendente { get; set; }

            [Required]
            [DisplayName("Importo riconosciuto")]
            [PraticheAzienda_ImportoContributo]
            public decimal? ImportoRiconosciuto { get; set; } = 0;

            public IEnumerable<Richiedente> Richiedenti { get; set; }
        }

        public List<Richiedente> ChildClass { get; set; }

        public class RichiedentiViewModel
        {
            public List<Richiedente> ChildClass { get; set; }

            public decimal? ImportoRiconosciutoTotale { get; set; }

            public bool? ReadOnly { get; set; }
        }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Inserire almeno un Richiedente")]
        public int? RichiedentiTotale { get; set; }

    }

    public class PraticheAzienda_EventiEccezionaliCalamitaNaturali : PraticheAzienda_BaseClass
    {
        [Required]
        [PraticheAzienda_EventiEccezionaliCalamitaNaturaliDataEventoValidation(ErrorMessage = "Data dell' evento non valida")]
        [DisplayName("Data dell'evento")]
        public DateTime? DataEvento { get; set; }

        [Required]
        [DisplayName("Danni strutture/attrezzature")]
        public decimal? TotaleDanniStruttureAttrezzature { get; set; }

        [Required]
        [DisplayName("Danni alle scorte")]
        public decimal? TotaleDanniScorte { get; set; }

        [Required]
        [PraticheAzienda_EventiEccezionaliCalamitaNaturaliImportoRiconosciutoValidation(ErrorMessage = "Importo riconoscito non valido")]
        [DisplayName("Importo riconosciuto")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoRiconosciuto { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }


    }

    public class PraticheAzienda_FormazioneAggiornamentoProfessionale : PraticheAzienda_BaseClass
    {
        [Required]
        [DisplayName("Tipo Partecipazione")]
        public int? MotivoRichiestaId { get; set; }

        [Required]
        [CustomRangeValidator(MinValue = "100",
            MinValueErrorMessage = "Non è possibile fare richiesta per importi inferiori a € 100,00")]
        [DisplayName("Costo del corso")]
        public decimal? TotaleFatture { get; set; }
      
        [Required]
        [RequiredFromEBLIGAdmin(ErrorMessage = "Il campo Importo totale rimborsato e obbligatorio")]
        [CustomRangeValidator(MaxValue = "520",
            MaxValueErrorMessage = "Non è possibile rimborsare importi superiori a € 520", ValidateOnlyForRoles = "Admin")]
        [DisplayName("Importo totale rimborsato")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoTotaleRimborsato { get; set; } = 0;
    }


    public class PraticheAzienda_QualitaInnovazione : PraticheAzienda_BaseClass
    {
        [Required]
        [DisplayName("Motivo della richiesta")]
        public int? MotivoRichiestaId { get; set; }

        [Required]
        [CustomRangeValidator(MinValue = "500",
            MinValueErrorMessage = "Non è possibile richiedere la prestazione per importi inferiori a € 500,00")]
        [DisplayName("Totale delle fatture")]
        public decimal? TotaleFatture { get; set; }

        [Required]
        [RequiredFromEBLIGAdmin(ErrorMessage = "Il campo Totale delle fatture accettate e obbligatorio")]
        [CustomRangeValidator(MinValue = "500",
            MinValueErrorMessage = "Non è possibile fare richiesta per importi inferiori a € 500,00", ValidateOnlyForRoles = "Admin")]
        [DisplayName("Totale delle fatture accettate")]
        public decimal? TotaleFatturAccettate { get; set; } = 0;

        [Required]
        [RequiredFromEBLIGAdmin(ErrorMessage = "Il campo Importo totale rimborsato e obbligatorio")]
        [CustomRangeValidator(MaxValue = "7500",
            MaxValueErrorMessage = "Non è possibile rimborsare importi superiori a € 7.500,00", ValidateOnlyForRoles = "Admin")]
        [DisplayName("Importo totale rimborsato")]
        [PraticheAzienda_ImportoContributo]
        public decimal? ImportoTotaleRimborsato { get; set; } = 0;
    }

    #endregion

    #region dipendente

    public class PraticheAzienda_Dipendente_Maternita : PraticheAzienda_BaseClass
    {
        [DisplayName("Codice Fiscale")]
        [Required]
        [VerificaTipoRichiestaUnivocoCodiceFiscaleValidator(ErrorMessage = "Per il Codice Fiscale e già stata presentata una richiesta", NomeCampo = "CodiceFiscale", Unica = true)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = false)]
        [MaxLength(16)]
        public string CodiceFiscale { get; set; }
    }

    public class PraticheAzienda_Dipendente_ContributoIscrizioneAsilo : PraticheAzienda_BaseClass
    {
        public PraticheAzienda_Dipendente_ContributoIscrizioneAsilo()
        {
            ChildClass = new List<PraticheAzienda_Dipendente_Parentela>();
        }

        public List<PraticheAzienda_Dipendente_Parentela> ChildClass { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Inserire almeno un Parente")]
        public int? Parenti { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool StatoFamigliaComposto { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool ProprioFiglio { get; set; }

        [Required]
        [DisplayName("Nome Cognome Figlio")]
        [MaxLength(100)]
        public string NomeCognomeFiglio { get; set; }

        [Required]
        [DisplayName("Nome del Asilo nido/materna")]
        [MaxLength(100)]
        public string AsiloNome { get; set; }

        [Required]
        [DisplayName("Indirizzo del Asilo nido/materna")]
        [MaxLength(100)]
        public string AsiloIndirizzo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Luogo { get; set; }

        [Required]
        public DateTime? Data { get; set; }
    }

    public class PraticheAzienda_Dipendente_Parentela
    {
        [Required]
        [DisplayName("Tipo Parentela")]
        public int? ParentelaId { get; set; }

        [Required]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = false)]
        [DisplayName("Codice Fiscale")]
        public string CodiceFiscale { get; set; }

        [Required]
        [DisplayName("Cognome e Nome")]
        public string CognomeNome { get; set; }

        [Required]
        [DisplayName("Luogo nascita")]
        public string LuogoNascita { get; set; }

        [Required]
        [DisplayName("Data nascita")]
        public DateTime? DataNascita { get; set; }

        public string TipoParentela { get; set; }

        [NotMapped]
        public class PraticheAzienda_Dipendente_Parenti
        {
            public List<PraticheAzienda_Dipendente_Parentela> ChildClass { get; set; }

            public bool? ReadOnly { get; set; }

        }
    }

    public class PraticheAzienda_Dipendente_ContributoIscrizioneScuolaMaterna : PraticheAzienda_BaseClass
    {
        public PraticheAzienda_Dipendente_ContributoIscrizioneScuolaMaterna()
        {
            ChildClass = new List<PraticheAzienda_Dipendente_Parentela>();
        }

        public List<PraticheAzienda_Dipendente_Parentela> ChildClass { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Inserire almeno un Parente")]
        public int? Parenti { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool StatoFamigliaComposto { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool ProprioFiglio { get; set; }

        [Required]
        [DisplayName("Nome Cognome Figlio")]
        [MaxLength(100)]
        public string NomeCognomeFiglio { get; set; }

        [Required]
        [DisplayName("Nome del Asilo nido/materna")]
        [MaxLength(100)]
        public string AsiloNome { get; set; }

        [Required]
        [DisplayName("Indirizzo del Asilo nido/materna")]
        [MaxLength(100)]
        public string AsiloIndirizzo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Luogo { get; set; }

        [Required]
        public DateTime? Data { get; set; }
    }

    public class PraticheAzienda_Dipendente_CarenzaMalattia : PraticheAzienda_BaseClass
    {
        public PraticheAzienda_Dipendente_CarenzaMalattia()
        {
            ChildClass = new List<EventiMalattia>();
        }

        public class EventiMalattia
        {
            [Required]
            [Range(3, int.MaxValue, ErrorMessage = "Ogni evento deve avere almeno 3 giorni")]
            public int? Giorni { get; set; }
            public DateTime? Data { get; set; }
        }

        public List<EventiMalattia> ChildClass { get; set; }

        [Required]
        [Range(2, int.MaxValue, ErrorMessage = "Inserire almeno due eventi di assenza per malattia")]
        public int? AssenzaMalattia { get; set; }

        public class PraticheAzienda_Dipendente_CarenzaMalattia_Eventi
        {
            public List<EventiMalattia> ChildClass { get; set; }

            public bool? ReadOnly { get; set; }
        }
    }

    #endregion

    #endregion




}
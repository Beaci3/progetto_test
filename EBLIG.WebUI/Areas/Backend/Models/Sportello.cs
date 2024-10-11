using EBLIG.DOM.Entitys;
using EBLIG.WebUI.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class SportelloRicercaModel
    {
        [MaxLength(75)]
        public string SportelloRicercaModel_RagioneSociale { get; set; }

        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale non è valido", Required = false, RequiredPivaOrCF = true)]
        public string SportelloRicercaModel_CodiceFiscalePartitaIva { get; set; }

        public string SportelloRicercaModel_Comune { get; set; }

        public int? SportelloRicercaModel_ComuneId { get; set; }

        public IEnumerable<Comuni> Comuni { get; set; }

        public string Ordine { get; set; } = "RagioneSociale asc";

        public int PageSize { get; set; } = 10;
    }

    public class SportelloRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Sportello> Result { get; set; }

        public SportelloRicercaModel Filtri { get; set; }
    }

    public class SportelloViewModel: Sportello
    {
        public bool? InformazioniPersonaliCompilati { get; set; }

        public bool? ReadOnly { get; set; }
    
        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo CF / P.Iva non è valido", Required = true, RequiredPivaOrCF = true)]
        public new string CodiceFiscalePIva { get; set; }

    }
}
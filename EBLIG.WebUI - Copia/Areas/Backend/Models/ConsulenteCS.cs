using EBLIG.DOM.Entitys;
using EBLIG.WebUI.ValidationAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class ConsulenteCsRicercaModel
    {
        [MaxLength(75)]
        public string ConsulenteCsRicercaModel_RagioneSociale { get; set; }

        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale non è valido", Required = false, RequiredPivaOrCF = true)]
        public string ConsulenteCsRicercaModel_CodiceFiscalePartitaIva { get; set; }

        public string ConsulenteCsRicercaModel_Comune { get; set; }

        public int? ConsulenteCsRicercaModel_ComuneId { get; set; }

        public IEnumerable<Comuni> Comuni { get; set; }

        public string Ordine { get; set; } = "RagioneSociale asc";

        public int PageSize { get; set; } = 10;
    }

    public class ConsulenteCsRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<ConsulenteCS> Result { get; set; }

        public ConsulenteCsRicercaModel Filtri { get; set; }
    }

    public class ConsulenteCSViewModel: ConsulenteCS
    {
        public bool? InformazioniPersonaliCompilati { get; set; }

        public bool? ReadOnly { get; set; }
    
        [MaxLength(16)]
        [ChecksumCFPiva(ErrorMessage = "Il campo CF / P.Iva non è valido", Required = true, RequiredPivaOrCF = true)]
        public new string CodiceFiscalePIva { get; set; }

    }
}
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class ComuniModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Comuni> Result { get; set; }

        public ComuniModel Filtri { get; set; }
    }

    public class ComuniModel
    {
        public int ComuneId { get; set; }
        public string CodCom { get; set; }
        public string DenCom { get; set; }
        public string SigPro { get; set; }
        public string CodSta { get; set; }
        public string DenPro { get; set; }
    }

    public class InsComuni
    {
        public int ComuneId { get; set; }
        [Required]
        [DisplayName("Codice Comune")]
        [RegularExpression("^[A-Za-z][0-9]{3}$", ErrorMessage = "Inserire un Codice Comune valido")]
        public string CodCom { get; set; }
        [Required]
        [DisplayName("Denominazione")]
        public string DenCom { get; set; }
        public string SigPro { get; set; }
        [Required]
        [DisplayName("Sigla Stato")]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "Inserire una Sigla Stato valida")]
        public string CodSta { get; set; }
        public string DenPro { get; set; }
        public int? ProvinciaId { get; set; }
        public int? RegioneId { get; set; }
        public int? CodReg { get; set; }
    }

    public class ComuniRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "DenCom";
    }


}
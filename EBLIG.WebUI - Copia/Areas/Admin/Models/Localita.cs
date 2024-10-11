using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class LocalitaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Localita> Result { get; set; }

        public LocalitaModel Filtri { get; set; }
    }

    public class LocalitaModel
    {
        public int LocalitaId { get; set; }
        public string Cap { get; set; }
        public string DenLoc { get; set; }
        public string SigPro { get; set; }
        public string CodCom { get; set; }
        public string DenCom { get; set; }
    }

    public class InsLocalita
    {
        public int LocalitaId { get; set; }
        [Required]
        [DisplayName("CAP")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Inserire un CAP valido")]
        public string Cap { get; set; }
        [Required]
        [DisplayName("Denominazione")]
        public string DenLoc { get; set; }
        [Required]
        [DisplayName("Sigla Provincia")]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "Inserire una Provincia valida")]
        public string SigPro { get; set; }
        public string CodCom { get; set; }
        public string DenCom { get; set; }
        public int? ProvinciaId { get; set; }
        public int? ComuneId { get; set; }
        public int? RegioneId { get; set; }
        public int? CodReg { get; set; }
        public string DenPro { get; set; }
    }

    public class LocalitaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "DenLoc";
    }


}
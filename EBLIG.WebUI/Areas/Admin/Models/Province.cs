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
    public class ProvinceModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Province> Result { get; set; }

        public ProvinceModel Filtri { get; set; }
    }

    public class ProvinceModel
    {
        public int ProvinciaId { get; set; }
        public string CodReg { get; set; }
        public string DenPro { get; set; }
        public string SigPro { get; set; }
    }

    public class InsProvince
    {
        public int ProvinciaId { get; set; }
        [Required]
        [DisplayName("Codice Provincia")]
        public int CodReg { get; set; }
        [Required]
        [DisplayName("Denominazione")]
        public string DenPro { get; set; }
        public string SigPro { get; set; }
        public string CodSta { get; set; }
        public int? RegioneId { get; set; }

        public IEnumerable<Regioni> Regioni { get; set; }
    }

    public class ProvinceRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "DenPro";
    }


}
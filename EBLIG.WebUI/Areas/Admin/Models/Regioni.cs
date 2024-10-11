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
    public class RegioniModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Regioni> Result { get; set; }

        public RegioniModel Filtri { get; set; }
    }

    public class RegioniModel
    {
        public int RegioneId { get; set; }
        public string CodReg { get; set; }
        public string DenReg { get; set; }
    }

    public class InsRegioni
    {
        public int RegioneId { get; set; }
        //[Required]
        //[DisplayName("Codice Regione")]
        //public int CodReg { get; set; }
        [Required]
        [DisplayName("Denominazione")]
        public string DenReg { get; set; }
    }

    public class RegioniRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "DenReg";
    }


}
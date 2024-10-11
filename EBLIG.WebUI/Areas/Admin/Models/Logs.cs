using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class LogsRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Data desc";
    }

    public class LogsRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Logs> Result { get; set; }

        public LogsRicercaModel Filtri { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class NavigationHistoryRicercaModel
    {
        public string Browsername{ get; set; }

        public IEnumerable<string> Browser { get; set; }

        public string Username{ get; set; }
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Data desc";
    }

    public class NavigationHistoryRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<NavigatioHistory> Result { get; set; }

        public NavigationHistoryRicercaModel Filtri { get; set; }
    }
}
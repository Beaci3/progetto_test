using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class AvvisoUtenteRicercaModel
    {
    }

    public class AvvisoUtenteRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<AvvisoUtente> Result { get; set; }

        public AvvisoUtenteRicercaModel Filtri { get; set; }
    }

    public class AvvisoUtenteViewModel : AvvisoUtente
    {
        public class RuoliAvviso
        {
            public string Nome { get; set; }

            public bool Checked { get; set; }
        }

        public RuoliAvviso[] Ruolo { get; set; }
    }

}
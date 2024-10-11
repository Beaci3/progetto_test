using EBLIG.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class IndirizzoViewModel
    {
        public bool? ReadOnly { get; set; }

        public int? RegioneId { get; set; }

        public int? ProvinciaId { get; set; }

        public int? LocalitaId { get; set; }

        public int? ComuneId { get; set; }
        public string Indirizzo { get; set; }

        public string Sigpro { get; set; }
        public string Codcom { get; set; }
        public int? Codreg { get; set; }

        public string RegioneElementNome { get; set; } = "Regione";
        public string RegioneElement { get; set; }

        public string ProvinciaElementNome { get; set; } = "Provincia";
        public string ProvinciaElement { get; set; }

        public string LocalitaElementNome { get; set; } = "Località";
        public string LocalitaElement { get; set; }

        public string ComuneElementNome { get; set; } = "Comune";
        public string ComuneElement { get; set; }

        public string IndirizzoElementNome { get; set; } = "Indirizzo";
        public string IndirizzoElement { get; set; }

        public Province Provincia { get; set; }

        public Regioni Regione { get; set; }

        public Comuni Comune { get; set; }

        public Localita Localita { get; set; }

        public bool? IncludiRegioni { get; set; } = false;

        public bool? IncludiEstero { get; set; } = false;

        public bool? ShowLocalita { get; set; } = true;

        public bool? ShowIndirizzo{ get; set; } = true;

        public int? Col { get; set; }
    }
}
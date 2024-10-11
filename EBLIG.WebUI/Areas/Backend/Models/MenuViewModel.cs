using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class MenuViewModel
    {
        public int Codmenu { get; set; }
        public int? CodmenuPadre { get; set; }
        public string Descrizione { get; set; }
        public string IconFa { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int? Ordine { get; set; }
        public bool Visible { get; set; }

        public string Area { get; set; }

        public string[] Ruoli { get; set; }

    }

}
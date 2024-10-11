using EBLIG.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class AllegatiClass
    {
        public string Nome { get; set; }
    }
    
    public class InsAllegati
    {
        [Required]
        public string Nome { get; set; }
    }

    public class EliminaAllegato
    {
        public int allegatoId { get; set; }
    }
}
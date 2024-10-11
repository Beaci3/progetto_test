using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("Tipologia")]
    public class Tipologia
    {
        public int TipologiaId { get; set; }
      
        [MaxLength(500)]
        public string Descrizione { get; set; }

        public bool? Partesociale { get; set; }
    }
}

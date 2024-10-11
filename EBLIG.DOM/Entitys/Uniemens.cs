using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("Uniemens")]
    public class Uniemens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UniemensId { get; set; }

        public int ID_EBT { get; set; }

        public int AziendaId { get; set; }
        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }

        public int? Anno { get; set; }

        public int? Mensilita { get; set; }

        //public decimal? TotaleEntrate { get;set; }

        //public decimal? TotaleMovimenti { get;set; }

        //public DateTime? DataUpdate { get; set; }

        //[MaxLength(50)]
        //public string OID { get; set; }

        public string UniemensBson { get; set; }
    }
}

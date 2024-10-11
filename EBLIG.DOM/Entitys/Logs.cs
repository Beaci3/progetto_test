using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("Logs")]
    public class Logs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogsId { get; set; }

        public DateTime Data { get; set; }

        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(50)]
        public string Ruolo { get; set; }

        public string ViewDataJson { get; set; }

        [MaxLength(255)]
        public string Model { get; set; }

        public string Message { get; set; }

        [MaxLength(255)]
        public string Action { get; set; }
    }
}

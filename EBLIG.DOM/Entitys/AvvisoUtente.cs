using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("AvvisoUtente")]
    public class AvvisoUtente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AvvisoUtenteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titolo { get; set; }

        [Required]
        public string Messaggio { get; set; }

        [Required]
        public DateTime DataInserimento { get; set; }

        public DateTime? DataScadenza{ get; set; }

        [InverseProperty("AvvisoUtente")]
        public virtual ICollection<AvvisoUtenteRuoli> AvvisoUtenteRuoli { get; set; }

        public bool? Popup { get; set; }

    }

    [Table("AvvisoUtenteRuoli")]
    public class AvvisoUtenteRuoli
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AvvisoUtenteRuoliId { get; set; }

        public int AvvisoUtenteId { get; set; }

        [ForeignKey("AvvisoUtenteId")]
        public virtual AvvisoUtente AvvisoUtente { get; set; }

        public string Ruolo { get; set; }
    }
}

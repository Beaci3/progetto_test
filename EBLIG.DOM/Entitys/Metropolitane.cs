using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{

    //  Gestione Tabelle >> Metropoliotane <<

    [Table("Regioni")]
    public class Regioni
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegioneId { get; set; }

        public int CODREG { get; set; }

        public string DENREG { get; set; }
        public DateTime? ULTAGG { get; set; }

        public string UTEAGG { get; set; }
    }

    [Table("Province")]
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProvinciaId { get; set; }

        public string SIGPRO { get; set; }

        public int CODREG { get; set; }

        public string DENPRO { get; set; }
        public DateTime? ULTAGG { get; set; }

        public string UTEAGG { get; set; }
    }

    [Table("Comuni")]
    public class Comuni
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComuneId { get; set; }

        public string CODCOM { get; set; }

        public string DENCOM { get; set; }

        public string SIGPRO { get; set; }

        public string CODSTA { get; set; }

        public DateTime? ULTAGG { get; set; }

        public string UTEAGG { get; set; }
    }

    [Table("Localita")]
    public class Localita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocalitaId { get; set; }

        public int CODLOC { get; set; }

        public string CAP { get; set; }

        public string DENLOC { get; set; }

        public string SIGPRO { get; set; }

        public string CODCOM { get; set; }

        public DateTime? ULTAGG { get; set; }

        public string UTEAGG { get; set; }  
    }

    //  Fine elenco

}

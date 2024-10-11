using EBLIG.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class TipoRichiesteRicercaModel
    {
        public int? Anno { get; set; }
        public string AbilitatoNuovaRichiesta { get; set; }
        public string IsTipoRichiestaDipendente { get; set; }

        public string Desrcizione { get; set; }
    }


    public class AllegatiModel
    {
        public int TipoRichiestaId { get; set; }
        public IEnumerable<Allegati> Allegati { get; set; }
    }

    public class Allegati
    {
        public bool Selezionato { get; set; }
        public bool Obbligatorio { get; set; }
        public string Nome { get; set; }
        public int AllegatoId{ get; set; }
        public int TipoRichiestaId { get; set; }
    }

    public class Duplica
    {
        [RegularExpression("^(19|20)\\d{2}$", ErrorMessage = "Inserire un anno valido")]
        public int Anno { get; set; }
        public int TipoRichiestaId { get; set; }
    }    
    
    public class Elimina
    {
        public int TipoRichiestaId { get; set; }
    }
}
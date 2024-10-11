using System.Collections.Generic;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class ChartDataModel
    {
        public int Order { get; set; }
        public string Label { get; set; }
        public int Data { get; set; }
        public string Color { get; set; }
    }

    public class ChartModel
    {
        public string ChartTitle { get; set; } 

        public IEnumerable<ChartDataModel>  ChartData{ get; set; }

    }

    public class Statistiche
    {
        public int? Totale { get; set; }

        public string Descrizione { get; set; }
    }


}
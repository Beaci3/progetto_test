using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Entitys
{
    [Table("NavigatioHistory")]
    public class NavigatioHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NavigatioHistoryId { get; set; }

        [MaxLength(75)]
        public string Username { get; set; }

        public DateTime? Data { get; set; }

        [MaxLength(75)]
        public string UserHostAddress { get; set; }

        public string CurrentUrl { get; set; }

        public bool? BrowserIsMobileDevice { get; set; }

        public int? BrowserJScriptVersionMajor { get; set; }
        
        public int? BrowserJScriptVersionMinor { get; set; }
        
        public int? BrowserMajorVersion { get; set; }
        
        [MaxLength(255)]
        public string BrowserMobileDeviceModel { get; set; }
        
        [MaxLength(255)]
        public string BrowserName { get; set; }
        
        [MaxLength(255)]
        public string BrowserVersion { get; set; }
    }
}

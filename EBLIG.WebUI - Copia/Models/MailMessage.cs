using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Models
{
    public class SimpleMailMessage
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string CcName { get; set; }
        public string CcEmail{ get; set; }
        public string BccName { get; set; }
        public string BccEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string FromEmail { get; set; }
        public string FromName { get; set; }

    }
}
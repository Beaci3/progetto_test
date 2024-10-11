using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Admin.Models
{
    public class RagioneSocialeConfigModel
    {
        [Required]
        public int? EnteId { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Regione { get; set; }

        [MaxLength(2)]
        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Indirizzo { get; set; }

        [Required]
        public string Citta { get; set; }

        [MaxLength(5)]
        [Required]
        public string Cap { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string Fax { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string PartitaIva { get; set; }


        [Required]
        public string SitoWeb { get; set; }

        [Required]
        public string UriPortale { get; set; }

        //[Required]
        //public string CodiceFiscale { get; set; }

        [EmailAddress]
        [Required]
        public string Pec { get; set; }
    }

    public class SepaConfigModel
    {

        [Required]
        public string MsgId { get; set; }


        [Required]
        public string InitgPty_Nm { get; set; }


        [Required]
        public string InitgPty_OrgId_Id { get; set; }


        [Required]
        public string InitgPty_OrgId_Issr { get; set; }


        [Required]
        public string ClrSysMmbId_MmbId { get; set; }


        [Required]
        public string PmtInf_PmtInfId { get; set; }


        [Required]
        public string PmtInf_PmtMtd { get; set; }


        [Required]
        public string PmtInf_ChrgBr { get; set; }


        [Required]
        public string PmtInf_PmtTpInf_SvcLvl_Cd { get; set; }


        [Required]
        public string PmtInf_DbtrAcct_Iban { get; set; }


        [Required]
        public string PmtInf_Dbtr_Nm { get; set; }


        [Required]
        public string PmtInf_Dbtr_PstlAdr_StrtNm { get; set; }


        [Required]
        public string PmtInf_Dbtr_PstlAdr_Ctry { get; set; }


        [Required]
        public string PmtInf_Dbtr_PstlAdr_PstCd { get; set; }


        [Required]
        public string PmtInf_Dbtr_PstlAdr_TwnNm { get; set; }


        [Required]
        public string PmtInf_DbtrAgt_FinInstnId_ClrSysMmbId_MmbId { get; set; }


        [Required]
        public string PmtInf_CdtTrfTxInf_PmtTpInf_CtgyPurp_Cd { get; set; }


        [Required]
        public string PmtInf_CdtTrfTxInf_PmtId_EndToEndId { get; set; }


        [Required]
        public string PmtInf_CdtTrfTxInf_Purp_Cd { get; set; }


        [Required]
        public string PmtInf_CdtTrfTxInf_RmtInf_Ustrd { get; set; }
    }

    public class TestMailSettingConfigModel
    {
        [Required]
        [EmailAddress]
        public string EmailTo { get; set; }

        [Required]
        public string Oggetto { get; set; }

        [Required]
        public string Messaggio { get; set; }
    }

    public class MailSettingConfigModel
    {
        [Required]
        public string SmtpServer { get; set; }

        [Required]
        public int SmtpServerPort { get; set; }

        [Required]
        public string SmtpServerUsername { get; set; }

        [Required]
        public string SmtpServerPassword { get; set; }

        //public string SmtpServerSenderEmail { get; set; }
        [Required]
        public bool SmtpServerAutentication { get; set; }

        [Required]
        public bool SmtpServerUseSSL { get; set; }

        [Required]
        public string FromName { get; set; }

        [Required]
        public string FromEmail { get; set; }
    }

    public class FTPConfigModel
    {
        [RegularExpression("^(ftp:\\/\\/)[a-z0-9.\\/-_]*\\)?$", ErrorMessage = "Indirizzo Ftp non valido")]
        [Required]
        [DisplayName("Indirizzo Ftp")]
        public string Path { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
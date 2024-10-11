using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Caching;
using System.Text;

namespace EBLIG.Utils
{
    public class ConfigurationProvider
    {
        static private ConfigurationProvider _instance = null;

        public Encoding EncodingInfo
        {
            get
            {
                switch (ConfigurationManager.AppSettings["Encoding"])
                {
                    case "ASCII":
                        return Encoding.ASCII;
                    case "Default":
                        return Encoding.Default;
                    case "Unicode":
                        return Encoding.Unicode;
                    case "UTF32":
                        return Encoding.UTF32;
                    case "UTF7":
                        return Encoding.UTF7;
                    case "UTF8":
                        return Encoding.UTF8;
                    default:
                        return Encoding.Default;
                }
            }

            private set { }
        }

        static public ConfigurationProvider Instance
        {
            get
            {
                if (_instance == null) _instance = new ConfigurationProvider();
                return _instance;
            }
        }

        public ConfigurationViewData GetConfigurationFromFile(string file)
        {
            try
            {
                using (var _jsonFile = new System.IO.StreamReader(file, EncodingInfo))
                {
                    var _json = JsonConvert.DeserializeObject<ConfigurationViewData>(_jsonFile.ReadToEnd(), new JsonSerializerSettings
                    {
                        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        //Formatting = Formatting.Indented,
                    });

                    return _json;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ConfigurationViewData GetConfigurationFromFile()
        {
            try
            {
                using (var _jsonFile = new System.IO.StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Configuration.json"), EncodingInfo))
                {
                    var _json = JsonConvert.DeserializeObject<ConfigurationViewData>(_jsonFile.ReadToEnd(), new JsonSerializerSettings
                    {
                        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        //Formatting = Formatting.Indented,
                    });

                    return _json;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void CacheConfiguration()
        {
            try
            {

                var memoryCache = MemoryCache.Default;

                if (memoryCache.Contains("GetConfiguration"))
                {
                    memoryCache.Remove("GetConfiguration");
                }

                using (var _jsonFile = new System.IO.StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Configuration.json"), EncodingInfo))
                {
                    var _json = JsonConvert.DeserializeObject<ConfigurationViewData>(_jsonFile.ReadToEnd(), new JsonSerializerSettings
                    {
                        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        //Formatting = Formatting.Indented,
                    });

                    var expiration = DateTimeOffset.UtcNow.AddMinutes(30);
                    memoryCache.Add("GetConfiguration", _json, expiration);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ConfigurationViewData GetConfiguration()
        {
            try
            {
                var memoryCache = MemoryCache.Default;

                if (!memoryCache.Contains("GetConfiguration"))
                {
                    CacheConfiguration();
                }

                return (ConfigurationViewData)memoryCache.Get("GetConfiguration");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveConfiguration(ConfigurationViewData c)
        {
            try
            {
                File.WriteAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Configuration.json"), JsonConvert.SerializeObject(c, Formatting.Indented), EncodingInfo);
                CacheConfiguration();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                ConfigurationViewData c = new ConfigurationViewData
                {
                    MailSetting = new MailSetting
                    {
                        SmtpServer = "smtp.gmail.com",
                        SmtpServerPort = 587,
                        SmtpServerUsername = "g.tuzzolino@sediin.it",
                        SmtpServerPassword = "W3lcometoG",
                        //SmtpServerSenderEmail = "g.tuzzolino@sediin.it",
                        SmtpServerAutentication = true,
                        SmtpServerUseSSL = false,
                        FromName = "EBLIG - Ente Bilaterale Artigianato Ligure",
                        FromEmail = "g.tuzzolino@sediin.it"
                    },
                    RegioneId = 8,
                    UploadFolder = "C:\\temp\\eblig\\",
                };

                //c.MailSetting.Body = new List<MailSetting.MailBodyModel>();
                //c.MailSetting.Body.Add(new MailSetting.MailBodyModel
                //{
                //    FromEmail = ""
                //});

                Sepa sepa = new Sepa();
                sepa.MsgId = "L-EBLIG";
                sepa.InitgPty_Nm = "EBLIG LOMBARDIA";
                sepa.InitgPty_OrgId_Id = "1026017E";
                sepa.InitgPty_OrgId_Issr = "CBI";
                sepa.ClrSysMmbId_MmbId = "07110";
                sepa.PmtInf_PmtInfId = "P-EBLIG";
                sepa.PmtInf_PmtMtd = "TRF";
                sepa.PmtInf_ChrgBr = "SLEV";
                sepa.PmtInf_PmtTpInf_SvcLvl_Cd = "SEPA";
                sepa.PmtInf_DbtrAcct_Iban = "IT57J0711003400000000006942";
                sepa.PmtInf_Dbtr_Nm = "EBLIG LOMBARDIA";
                sepa.PmtInf_Dbtr_PstlAdr_StrtNm = "VIA MIGUEL CERVANTES DE SAAVEDRA";
                sepa.PmtInf_Dbtr_PstlAdr_Ctry = "IT";
                sepa.PmtInf_Dbtr_PstlAdr_PstCd = "80133";
                sepa.PmtInf_Dbtr_PstlAdr_TwnNm = "Napoli";
                sepa.PmtInf_DbtrAgt_FinInstnId_ClrSysMmbId_MmbId = "07110";
                sepa.PmtInf_CdtTrfTxInf_PmtTpInf_CtgyPurp_Cd = "SUPP";
                sepa.PmtInf_CdtTrfTxInf_PmtId_EndToEndId = "P-EBLIG";
                sepa.PmtInf_CdtTrfTxInf_Purp_Cd = "OTHR";
                sepa.PmtInf_CdtTrfTxInf_RmtInf_Ustrd = "P-EBLIG";

                c.Sepa = sepa;

                RagioneSociale ragioneSociale = new RagioneSociale();
                ragioneSociale.Nome = "Eblig";

                c.RagioneSociale = ragioneSociale;

                UniemensData uniemens = new UniemensData();
                uniemens.Colonna = new List<string>
                {
                    "FSBA (Azienda)",
                    "FSBA (Dipendenti)",
                    "Ebna (2016)"
                };

                c.Uniemens = uniemens;

                var ftp = new FTP();
                var filelist = new Dictionary<string, string>();
                filelist.Add("a", "b");
                filelist.Add("b", "a");

                //ftp.FileList = new Dictionary<string, string>();
                //ftp.FileList = filelist;
                //c.Ftp = ftp;

                var _json = JsonConvert.SerializeObject(c);

                //File.WriteAllText("Configuration.json", JsonConvert.SerializeObject(c));
                //File.WriteAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Configuration.json"), JsonConvert.SerializeObject(c));
                CacheConfiguration();
            }
            catch (Exception)
            {
                throw;
            }
        }


    }

    public class ConfigurationViewData
    {
        public int RegioneId { get; set; }

        public FTP Ftp { get; set; }

        public RagioneSociale RagioneSociale { get; set; }

        public Sepa Sepa { get; set; }

        public MailSetting MailSetting { get; set; }

        public string UploadFolder { get; set; }

        public UniemensData Uniemens { get; set; }
    }

    public class FTP
    {
        public string Path { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class MailSetting
    {
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string SmtpServerUsername { get; set; }
        public string SmtpServerPassword { get; set; }
        //public string SmtpServerSenderEmail { get; set; }
        public bool SmtpServerAutentication { get; set; }
        public bool SmtpServerUseSSL { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }

        //public List<MailBodyModel> Body { get; set; }

        //public class MailBodyModel
        //{
        //    public string Key { get; set; }
        //    public string Value { get; set; }
        //    public string FromName { get; set; }
        //    public string FromEmail { get; set; }
        //}

    }

    public class RagioneSociale
    {

        public int EnteId { get; set; }
        public string Nome { get; set; }
        public string Regione { get; set; }
        public string Provincia { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string Cap { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Pec { get; set; }
        public string PartitaIva { get; set; }
        public string SitoWeb { get; set; }
        public string UriPortale { get; set; }
        //public string CodiceFiscale { get; set; }
    }

    public class Sepa
    {
        public string MsgId { get; set; }
        public string InitgPty_Nm { get; set; }
        public string InitgPty_OrgId_Id { get; set; }
        public string InitgPty_OrgId_Issr { get; set; }
        public string ClrSysMmbId_MmbId { get; set; }
        public string PmtInf_PmtInfId { get; set; }
        public string PmtInf_PmtMtd { get; set; }
        public string PmtInf_ChrgBr { get; set; }
        public string PmtInf_PmtTpInf_SvcLvl_Cd { get; set; }
        public string PmtInf_DbtrAcct_Iban { get; set; }
        public string PmtInf_Dbtr_Nm { get; set; }
        public string PmtInf_Dbtr_PstlAdr_StrtNm { get; set; }
        public string PmtInf_Dbtr_PstlAdr_Ctry { get; set; }
        public string PmtInf_Dbtr_PstlAdr_PstCd { get; set; }
        public string PmtInf_Dbtr_PstlAdr_TwnNm { get; set; }
        public string PmtInf_DbtrAgt_FinInstnId_ClrSysMmbId_MmbId { get; set; }
        public string PmtInf_CdtTrfTxInf_PmtTpInf_CtgyPurp_Cd { get; set; }
        public string PmtInf_CdtTrfTxInf_PmtId_EndToEndId { get; set; }
        public string PmtInf_CdtTrfTxInf_Purp_Cd { get; set; }
        public string PmtInf_CdtTrfTxInf_RmtInf_Ustrd { get; set; }
    }

    public class UniemensData
    {
        public List<string> Colonna { get; set; }

        public decimal Scoperto { get; set; }
    }
}

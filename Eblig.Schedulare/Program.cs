using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Importer;
using EBLIG.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EBLIG.DOM.Importer.ImportProvider;

namespace Eblig.Schedulare
{
    internal class Program
    {
        static StringBuilder Logger;

        static string BackupPath = "import";

        static string Id { get; set; }

        static ConfigurationViewData Configuration
        {
            get
            {
                return ConfigurationProvider.Instance.GetConfigurationFromFile(ConfigurationManager.AppSettings["Configuration"]);
            }
        }

        static string PathTemp
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "temp");
            }
        }

        static string PathLogs
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "logs");
            }
        }

        static void Main(string[] args)
        {
            Logger = new StringBuilder();

            ProcessImport();

            for (int i = 0; i < 10; i++)
            {
                Console.Write("\rchiusura tra " + (10 - i).ToString() + " ");
                Console.SetCursorPosition(0, Console.CursorTop);
                Thread.Sleep(1000);
            }

            Environment.Exit(0);
        }

        private static void ProcessImport()
        {
            try
            {
                Id = DateTime.Now.Ticks.ToString();

                if (!DirectoryExist(BackupPath))
                {
                    CreateDirectory(BackupPath);
                }

                if (!DirectoryExist(BackupPath + "/" + Id))
                {
                    CreateDirectory(BackupPath + "/" + Id);
                }

                List<Task> tasks = new List<Task>();

                var _files = GetFiles();

                if (_files?.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ReportEsito($"{_files.Count} allegati validi trovati, preparazione download dei file in corso...");
                    ReportEsito("");

                    Console.ForegroundColor = ConsoleColor.White;

                    foreach (var item in _files)
                    {
                        tasks.Add(Task.Run(() => DownloadFile(item)));
                    }

                    Task.WhenAll(tasks).Wait();

                    Console.ForegroundColor = ConsoleColor.Green;
                    //inizio import
                    ReportEsito("");
                    ReportEsito("Download completato");
                    ReportEsito("");

                    Console.ForegroundColor = ConsoleColor.White;

                    ReportEsito("Preprazione importazione in corso...");
                    ReportEsito("");

                    foreach (var item in _files)
                    {
                        StartImport(item);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    ReportEsito("");
                    ReportEsito("Importazione conclusa");

                    if (!Directory.Exists(PathLogs))
                    {
                        Directory.CreateDirectory(PathLogs);
                    }

                    File.WriteAllText(Path.Combine(PathLogs, DateTime.Now.Ticks + ".txt"), Logger.ToString());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ReportEsito("Nessun file da importare");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ReportEsito(ex.Message);

                try
                {
                    UnitOfWork unitOfWork = new UnitOfWork();
                    unitOfWork.LogsRepository.Insert(new Logs
                    {
                        Data = DateTime.Now,
                        Ruolo = "Admin",
                        Username = "Windows",
                        Model = null,
                        ViewDataJson = null,
                        Message = "Errore Procedura schedulata ProcessImport: " + ex.Message,
                        Action = "Procedura schedulata"
                    });
                    unitOfWork.Save();
                }
                catch
                {

                }
            }
            finally
            {
                // Application.Exit();
            }
        }

        private static void StartImport(string fileName)
        {
            try
            {
                var _outpath = Path.Combine(PathTemp, fileName);

                var _ms = new MemoryStream(File.ReadAllBytes(_outpath));

                var _enum = GetEnum(Path.GetFileNameWithoutExtension(fileName));

                ImportProvider p = new ImportProvider
                {
                    Username = "Windows",
                    Ruolo = "Admin",
                    FileStream = _ms,
                    TipoImport = _enum.ToString(),
                    Anno = 0,
                };

                if (_enum == ImportKey.Uniemens)
                {
                    var _uniemenstext = Path.Combine(PathTemp, "uniemens.txt");
                    //download uniemens.txt che contiene anno
                    DownloadFile("uniemens.txt");
                    if (File.Exists(_uniemenstext))
                    {
                        var _lines = File.ReadLines(_uniemenstext);
                        foreach (var item in _lines)
                        {
                            if (item != null)
                            {
                                int.TryParse(item, out int anno);
                                if (anno == 0)
                                {
                                    continue;
                                }
                                p.Anno = anno;
                                break;
                            }
                        }
                    }

                    File.Delete(_uniemenstext);
                    MoveFile("uniemens.txt");
                    p.Anno = p.Anno == 0 ? DateTime.Now.Year : p.Anno;
                }

                p.OnReport += P_OnReport;
                p.OnErrorFile += P_OnErrorFile;
                p.ProcessImport();

                Console.ForegroundColor = ConsoleColor.Red;
                ReportEsito(_enum.ToString() + " importati");

                File.Delete(_outpath);
                MoveFile(fileName);

            }
            catch (Exception ex)
            {
                ReportEsito(ex.Message);
            }
        }

        private static List<string> GetFiles()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                ReportEsito("Attendere, conessione al server in corso...");

                var _config = Configuration;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_config.Ftp.Path);

                if (!string.IsNullOrWhiteSpace(_config.Ftp.Username) && !string.IsNullOrWhiteSpace(_config.Ftp.Password))
                {
                    request.Credentials = new NetworkCredential(_config.Ftp.Username, _config.Ftp.Password);
                }

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                ReportEsito("conessione effetuata, lettura contenuto della cartella (sono ammessi solo \".json\")...");
                ReportEsito("");

                Console.ForegroundColor = ConsoleColor.White;

                List<string> files = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var lineArr = line.Split('/');
                    line = lineArr[lineArr.Count() - 1];

                    if (Path.GetExtension(line)?.ToLower() == ".json")
                    {
                        var _enum = GetEnum(Path.GetFileNameWithoutExtension(line));

                        switch (_enum)
                        {
                            case ImportKey.ConsulenteCs:
                            case ImportKey.Aziende:
                            case ImportKey.Dipendenti:
                            case ImportKey.Uniemens:
                            case ImportKey.Coperture:
                                files.Add(line);
                                ReportEsito($"- {line}");
                                break;
                            default:
                                break;
                        }
                    }

                    line = streamReader.ReadLine();

                }

                streamReader.Close();

                ReportEsito("");

                return files;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void DownloadFile(string fileName)
        {
            try
            {
                if (!Directory.Exists(PathTemp))
                {
                    Directory.CreateDirectory(PathTemp);
                }

                var _outpath = Path.Combine(PathTemp, fileName);

                var _config = Configuration;

                Uri _uri = new Uri(new Uri(_config.Ftp.Path), fileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);

                if (!string.IsNullOrWhiteSpace(_config.Ftp.Username) && !string.IsNullOrWhiteSpace(_config.Ftp.Password))
                {
                    request.Credentials = new NetworkCredential(_config.Ftp.Username, _config.Ftp.Password);
                }

                request.Method = WebRequestMethods.Ftp.DownloadFile;
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(_outpath))
                {
                    ftpStream.CopyTo(fileStream);
                }

                Console.ForegroundColor = ConsoleColor.White;
                ReportEsito($"- allegato {fileName} scaricato nella cartella {_outpath}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ReportEsito($"- errore scaricare {fileName} - {ex.Message}");
            }
        }

        private static void MoveFile(string fileName)
        {
            try
            {
                var _config = Configuration;

                Uri _uri = new Uri(new Uri(_config.Ftp.Path), fileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);

                if (!string.IsNullOrWhiteSpace(_config.Ftp.Username) && !string.IsNullOrWhiteSpace(_config.Ftp.Password))
                {
                    request.Credentials = new NetworkCredential(_config.Ftp.Username, _config.Ftp.Password);
                }

                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = BackupPath + "/" + Id + "/" + fileName;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                }
            }
            catch (Exception ex)
            {
                //ReportEsito($"- errore eliminare {fileName} - {ex.Message}");
            }
        }

        private static void CreateDirectory(string path)
        {
            var _config = Configuration;

            Uri _uri = new Uri(new Uri(_config.Ftp.Path), path);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UseBinary = true;

            if (!string.IsNullOrWhiteSpace(_config.Ftp.Username) && !string.IsNullOrWhiteSpace(_config.Ftp.Password))
            {
                request.Credentials = new NetworkCredential(_config.Ftp.Username, _config.Ftp.Password);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
            }

        }

        static bool DirectoryExist(string path)
        {
            try
            {
                var _config = Configuration;

                Uri _uri = new Uri(new Uri(_config.Ftp.Path), path);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ReportEsito(string v)
        {
            Logger.AppendLine(v);
            Console.WriteLine(v);
        }

        private static void P_OnErrorFile(string base64, string tipo, string username, string ruolo)
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                unitOfWork.LogsRepository.Insert(new Logs
                {
                    Data = DateTime.Now,
                    Ruolo = ruolo,
                    Username = username,
                    Model = null,
                    ViewDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64)).Replace(Environment.NewLine, "<br/>"),
                    Message = "Errore Import " + tipo,
                    Action = "Procedura schedulata"
                });
                unitOfWork.Save();
            }
            catch
            {
            }
        }


        //qui
        private static void P_OnReport(string processoId, string username, string tipoImport, int index, int totale, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            //Console.Write(string.Format("\rpercentuale: {0}", "".PadLeft(Console.CursorLeft, ' ')));
            Console.Write(string.Format("\r{2} Elaborati: {0} / {1}", index, totale, tipoImport));
            Console.SetCursorPosition(0, Console.CursorTop);
            //Console.Write(string.Format("\r{0}",  message));
            //Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static ImportKey? GetEnum(string v)
        {
            try
            {
                var p = (ImportKey)Enum.Parse(typeof(ImportKey), v, true);
                return p;
            }
            catch
            {
                return null;
            }
        }
    }

    //internal class EqualityComparer : IEqualityComparer<string>
    //{
    //    public bool Equals(string x, string y)
    //    {
    //        return x?.ToUpper() == y?.ToUpper();
    //    }

    //    public int GetHashCode(string obj)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

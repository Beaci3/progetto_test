using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EBLIG.DOM.DAL;
using EBLIG.DOM.Entitys;
using EBLIG.DOM.Importer;
using EBLIG.Utils;
using static EBLIG.DOM.Providers.LiquidazioneIdProvider;

namespace EBLIG.DOM.Providers
{
    public delegate void OnSuccessSendMailLiquidazioneReport(string processoId, string username, string tipoImport, int index, int totale, string message);

    public delegate string OnSendMailLiquidazioneReport(SendMailLiquidazioneEmailResultModel model);//, HttpContext context);

    public delegate void OnErrorFileMailLiquidazioneReport(string base64, string tipo, string username, string ruolo);

    public class LiquidazioneIdProvider
    {
        public event OnSuccessSendMailLiquidazioneReport OnSuccessSendMailLiquidazioneReport;

        public event OnSendMailLiquidazioneReport OnSendMailLiquidazioneReport;

        public event OnErrorFileMailLiquidazioneReport OnErrorFileMailLiquidazione;

        public class SendMailLiquidazioneEmailResultModel
        {
            public bool IsDipendente { get; set; }

            public string Importo { get; set; }

            public string Ragionesociale { get; set; }

            public string Nominativo { get; set; }

            public string Email { get; set; }

            public string Iban { get; set; }

            public List<string> NominativiDipendenti { get; set; }
            public string Body { get; set; }
            public string TipoRichiesta { get; set; }
        }

        public List<string> ErrorList { get; set; }

        public string Ruolo { get; set; }

        public string Username { get; set; }

        public string BodyMailAzienda { get; set; }

        public string BodyMailDipendente { get; set; }

        public HttpContext CurrentHttpContext { get; set; }
        public void ProcessSendMailLiquidazione(int liquidazioneId)
        {

            // HttpContext httpContext = HttpContext.Current;
            ErrorList = new List<string>();

            var _id = Guid.NewGuid().ToString();

            try
            {

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, 0, "Attendere, preparazione dati in corso...");

                #region MyRegion

                UnitOfWork unitOfWork = new UnitOfWork();

                var _liquidazione = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId)?.FirstOrDefault();

                var _emailesito = _liquidazione.MailInviate.Where(x => x.Inviata == true);

                var _l = _liquidazione.LiquidazionePraticheRegionali.Select(x => x.PraticheRegionaliImprese);

                List<SendMailLiquidazioneEmailResultModel> _listEmail = new List<SendMailLiquidazioneEmailResultModel>();

                var _x = 0;

                //TODO
                //mail aziende per le suoi richieste
                //foreach (var item in _l.Where(x => x.TipoRichiesta.IsTipoRichiestaDipendente == true))
                //{
                //    SendMailLiquidazioneEmailResultModel _aziendamail = new SendMailLiquidazioneEmailResultModel
                //    {
                //        Importo = item.ImportoContributoNetto.GetValueOrDefault().ToString("n"),
                //        Iban = item.Iban.ToUpper().RemoveWhiteSpace(),
                //        Ragionesociale = item.Azienda?.RagioneSociale,
                //        Email = item.Azienda?.Email,
                //        IsDipendente = false,
                //        Body = BodyMailAzienda,
                //        TipoRichiesta = item.TipoRichiesta.Descrizione
                //    };


                //}


                foreach (var item in _l.Select(x => x.Iban.ToUpper().RemoveWhiteSpace()).Distinct())
                {

                    var _riga = _l.FirstOrDefault(x => x.Iban.ToUpper().RemoveWhiteSpace() == item.ToUpper().RemoveWhiteSpace());

                    var _pagamenti = _l.Where(x => x.Iban.ToUpper().RemoveWhiteSpace() == item.ToUpper().RemoveWhiteSpace());

                    var _sommaTotale = _pagamenti?.Sum(x => x.ImportoContributoNetto).GetValueOrDefault();

                    SendMailLiquidazioneEmailResultModel _aziendamail = new SendMailLiquidazioneEmailResultModel();
                    _aziendamail.NominativiDipendenti = new List<string>();
                    _aziendamail.Importo = _sommaTotale.GetValueOrDefault().ToString("n");
                    _aziendamail.Iban = item.ToUpper().RemoveWhiteSpace();
                    _aziendamail.Ragionesociale = _riga.Azienda?.RagioneSociale;
                    _aziendamail.Email = _riga.Azienda?.Email;
                    _aziendamail.IsDipendente = false;
                    _aziendamail.Body = BodyMailAzienda;
                    _aziendamail.TipoRichiesta = _riga.TipoRichiesta.Descrizione;

                    foreach (var pagamento in _pagamenti)
                    {
                        if (pagamento.TipoRichiesta.IsTipoRichiestaDipendente.GetValueOrDefault())
                        {
                            var _importo = pagamento.ImportoContributoNetto.GetValueOrDefault().ToString("n");
                            var _nominativo = $"{pagamento.Dipendente?.Cognome} {pagamento.Dipendente?.Nome}";
                            _aziendamail.NominativiDipendenti.Add($"{pagamento.Dipendente?.CodiceFiscale}, {_nominativo}");

                            _listEmail.Add(new SendMailLiquidazioneEmailResultModel
                            {
                                IsDipendente = true,
                                Email = pagamento.Dipendente?.Email,
                                Nominativo = _nominativo,
                                Ragionesociale = pagamento.Azienda?.RagioneSociale,
                                Importo = _importo,
                                Body = BodyMailDipendente,
                                TipoRichiesta = pagamento.TipoRichiesta.Descrizione
                            });
                        }
                        else
                        {

                        }
                    }

                    _listEmail.Add(_aziendamail);
                }

                #endregion

                var _totaleRighe = _listEmail.Count();

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, _totaleRighe, "Inizion invio mail in corso...");

                var _taskList = new List<Task>();

                var _inviata = false;

                var _mess = "";

                var _xx = 0;


                foreach (var item in _listEmail)
                {
                    try
                    {
                        if (_emailesito.FirstOrDefault(x => x.Email.ToUpper() == item.Email.ToUpper()) != null)
                        {
                            OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, $"Email già stato inviata {item.Email}");
                            continue;
                        }

                        _xx++;

                        _mess = OnSendMailLiquidazioneReport?.Invoke(item);

                        if (!string.IsNullOrWhiteSpace(_mess))
                        {
                            _inviata = false;
                            ErrorList.Add(_mess);
                        }
                        else
                        {
                            _inviata = true;
                            _mess = $"Mail inviata a {item.Email}";
                        }

                        LiquidazionePraticheRegionaliMailInviatiEsito _esito = new LiquidazionePraticheRegionaliMailInviatiEsito
                        {
                            LiquidazioneId = liquidazioneId,
                            Esito = _mess,
                            Inviata = _inviata,
                            Email = item.Email
                        };

                        unitOfWork.LiquidazionePraticheRegionaliMailInviatiEsitoRepository.Insert(_esito);
                        unitOfWork.Save(false);

                        OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, _mess);

                        if (_xx % 25 == 0)
                        {
                            OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, _mess + "<br/><span class='text-danger'>Attendere, attesa di 2 minuti</span>");
                            //attendi 2 minuti
                            Thread.Sleep(120000);
                        }
                        //_taskList.Add(Task.Run(() =>
                        //{

                        //}));
                    }
                    catch (Exception)
                    {

                    }
                }

                //Task.WhenAll(_taskList).Wait();

                UnitOfWork unitOfWork1 = new UnitOfWork();
                var _li = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId).FirstOrDefault();
                _li.MailDaInviareTotale = _listEmail.Count();
                unitOfWork.LiquidazioneRepository.Update(_li);
                unitOfWork.Save(false);

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", _totaleRighe, _totaleRighe, "Processo terminato");

                if (ErrorList.Count() > 0)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Join(Environment.NewLine, ErrorList));

                        var _base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
                        OnErrorFileMailLiquidazione?.Invoke(_base64, "SendMail", Username, Ruolo);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, 0, "Si e verificcato un errore, " + ex.Message);
            }
        }
    }
}

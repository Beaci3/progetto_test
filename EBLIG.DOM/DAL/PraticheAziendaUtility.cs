using EBLIG.DOM.Entitys;
using LambdaSqlBuilder;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace EBLIG.DOM.DAL
{
    public class PraticheAziendaUtility
    {

        public static decimal? GetImportoErogatoIncentiviCovid19Imprese(int giorni)
        {
            try
            {
                //Importo erogato (campo valorizzato automaticamente in base al numero di giorni di sospensione dichiarati.
                //Da 03 a 05 giorni di sospensione saranno erogati 500€,
                //da 06 a 10 giorni di sospensione 1.000€ e
                //oltre gli 11 giorni di sospensione 2.000€

                var _importo = 0;
                if (giorni <= 0 || giorni > 0 && giorni < 3)
                {
                    _importo = 0;
                }
                else if (giorni >= 3 && giorni <= 5)
                {
                    _importo = 500;
                }

                else if (giorni > 5 && giorni <= 10)
                {
                    _importo = 1000;
                }
                else
                {
                    _importo = 2000;
                }

                return _importo;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static decimal? GetImportoTotaleRimborsatoSicurezzaLavoroImprese(decimal importoAccettato)
        {
            try
            {
                //Importo totale rimborsato
                //20 % fino a € 10.000,00 -
                //5 % da € 10.000,01 a € 150.000
                //in base al campo “Totale delle fatture accettate”

                decimal _importoRimborsato = 0;
                if (importoAccettato > 150000)
                {
                    throw new Exception("Importo non valido");
                }

                if (importoAccettato > 10000)
                {
                    var _importoaccettatodif = importoAccettato > 10000 ? importoAccettato - 10000 : importoAccettato;
                    var _importoaccettatodiecimila = importoAccettato - _importoaccettatodif;

                    var _importoRimborsatodif = Math.Round((_importoaccettatodif / 100) * 5, 2);

                    _importoRimborsato = Math.Round((_importoaccettatodiecimila / 100) * 20, 2);

                    _importoRimborsato = _importoRimborsatodif + _importoRimborsato;
                }
                else
                {
                    _importoRimborsato = Math.Round((importoAccettato / 100) * 20, 2);
                }

                return _importoRimborsato;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static decimal? GetImportoTotaleRimborsatoQualitaInnovazioneImprese(decimal importoAccettato)
        {
            try
            {
                //Importo totale rimborsato (effettuare il seguente calcolo:
                //il 10% del campo “Totale delle fatture accettate")
                //Verificare che l'importo totale rimborsato non superi 7.500€
                //(in quel caso verrà impostato il massimo erogabile a 7.500€)
                //Non è possibile fare richiesta per importi inferiori a 500€)

                decimal _importoRimborsato = 0;

                var _percentuale = 10;

                if (importoAccettato < 500)
                {
                    throw new Exception("Importo non valido");
                }

                _importoRimborsato = Math.Round((importoAccettato / 100) * _percentuale, 2);

                if (_importoRimborsato > 7500)
                {
                    _importoRimborsato = 7500;
                }

                return _importoRimborsato;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static decimal? GetImportoTotaleRimborsatoFormazioneAggiornamentoProfessionale(decimal importoAccettato, int motivazioneRichiestaId)
        {
            try
            {
                //Il contributo è concesso nella misura del 30 % del costo di partecipazione al corso al netto di IVA e non
                //potrà superare l'importo di euro 200.
                //Nel caso di partecipazione di titolari, soci o collaboratori a iniziative formative contestualmente ai 
                //loro dipendenti previsto dall’accordo del 17 / 03 / 2008 sulla formazione professionale 
                //realizzata con Fondartigianato, resta confermato il contributo del 50 % del 
                //costo di partecipazione del corso con il contributo massimo di euro 520.

                UnitOfWork unitOfWork = new UnitOfWork();
                var _motivazione = unitOfWork.MotivazioniRichiestaRepository.Get(x => x.MotivazioniRichiestaId == motivazioneRichiestaId).FirstOrDefault();

                if (_motivazione == null)
                {
                    throw new Exception("Motivazione non valida");
                }

                decimal _importoRimborsato = 0;
                var _percentuale = _motivazione.PercentualeRimborso.GetValueOrDefault();

                if (importoAccettato < 100)
                {
                    return 0;
                }

                _importoRimborsato = Math.Round((importoAccettato / 100) * _percentuale, 2);

                if (_importoRimborsato > _motivazione.ImportoMaxRimborsato.GetValueOrDefault())
                {
                    _importoRimborsato = _motivazione.ImportoMaxRimborsato.GetValueOrDefault();
                }

                return _importoRimborsato;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static decimal? GetImportoEventiEccezionaliCalamitaNaturaliImprese(decimal? danniAttrezzature = 0, decimal? danniScorte = 0)
        {
            try
            {
                //Importo riconosciuto (Campo valorizzato automaticamente col seguente calcolo:
                //il 20% DEL il 100% del campo
                //“Totale in euro dei danni alle strutture/attrezzature”
                //+ l'80% del campo “Totale in euro dei danni alle scorte”)

                var _percentualeDanniAttrezzatura = 20;

                var _percentualeDanniScorte = 80;

                var _importoRimborsatoAttrezzatura = danniAttrezzature.GetValueOrDefault();// Math.Round((danniAttrezzature.GetValueOrDefault() / 100) * _percentualeDanniAttrezzatura, 2);

                var _importoRimborsatoScorte = Math.Round((danniScorte.GetValueOrDefault() / 100) * _percentualeDanniScorte, 2);

                var _totale = _importoRimborsatoAttrezzatura + _importoRimborsatoScorte;

                _totale = Math.Round((_totale / 100) * _percentualeDanniAttrezzatura, 2);

                if (_totale > 10000)
                {
                    _totale = 10000;
                }

                if (_totale < 0)
                {
                    _totale = 0;
                }

                return _totale;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static decimal? GetImportoIncrementoMantenimentoOccupazionImprese(int ore)
        {
            try
            {
                //•	Calcolo per la prestazione Incremento e mantenimento occupazione: impostare controllo sul campo
                //"Ore settimanali dipendente" che deve essere max 40. L'importo lordo massimo erogabile è di 1000 euro
                //che si proporziona in base alle ore lavorate.
                //Es: Ore settimanali dipendenti 20 - campo importo lordo 500euro.

                var _maxore = 40;

                if (ore > _maxore)
                {
                    ore = _maxore;
                }

                if (ore < 0)
                {
                    ore = 0;
                }

                var _importoMax = 1000;

                decimal _x = _importoMax / _maxore * ore;

                return Math.Round(_x, 2);
            }
            catch (Exception)
            {
                return 0m;
            }
        }

        public static bool VerificaTipoRichiestaUnivocoCodiceFiscale(int aziendaId, int tipoRichiestaId, string codiceFiscale, int richiestaId, string nomeCampo, bool? unica = true)
        {
            try
            {
                UnitOfWork u = new UnitOfWork();
                var _richieste = u.PraticheRegionaliImpreseRepository.Get(x =>
                x.PraticheRegionaliImpreseId != richiestaId
                && x.TipoRichiestaId == tipoRichiestaId
                && (unica != true ? x.AziendaId == aziendaId : true)
                && (x.StatoPraticaId != (int)EbligEnums.StatoPratica.Bozza
                && x.StatoPraticaId != (int)EbligEnums.StatoPratica.Annullata));
                //&& (x.StatoPraticaId == (int)EbligEnums.StatoPratica.Inviata
                //|| x.StatoPraticaId == (int)EbligEnums.StatoPratica.InviataRevisionata
                //|| x.StatoPraticaId == (int)EbligEnums.StatoPratica.Confermata));

                var _datiPratica = _richieste?.Select(x => x.DatiPratica.Where(d => d.Nome != null && d.Nome.ToUpper().EndsWith(nomeCampo.ToUpper())));

                foreach (var item in _datiPratica)
                {
                    foreach (var row in item)
                    {
                        if (row?.Valore?.ToLower() == codiceFiscale?.ToLower())
                        {
                            return true;
                        }
                    }
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

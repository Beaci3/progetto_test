using EBLIG.DOM.DAL;
using EBLIG.DOM.Models;
using EBLIG.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EBLIG.DOM.Providers
{
    public class SepaProvider
    {
        public string SepaBase64String(int liquidazioneId)
        {
            var base64 = Convert.ToBase64String(SepaStream(liquidazioneId).ToArray());
            return base64;
        }

        public string SepaXmlString(int liquidazioneId)
        {
            string v = Encoding.UTF8.GetString(SepaStream(liquidazioneId).ToArray());
            return v;
        }

        public MemoryStream SepaStream(int liquidazioneId)
        {
            ConfigurationProvider provider = new ConfigurationProvider();
            var _config = provider.GetConfiguration();

            var _meseAnno = $"{DateTime.Now.Month}-{DateTime.Now.Year}";

            UnitOfWork unitOfWork = new UnitOfWork();
            var _liquidazione = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId)?.FirstOrDefault();
            var _numLiquidazione = _liquidazione.LiquidazioneId.ToString().PadLeft(7, '0');
            var _l = _liquidazione.LiquidazionePraticheRegionali.Select(x => x.PraticheRegionaliImprese);

            //crea payment
            CBIPaymentRequest payment = new CBIPaymentRequest();

            //GrpHdr
            CBIPaymentRequestGrpHdr grpHdr = new CBIPaymentRequestGrpHdr();
            grpHdr.MsgId = $"{_config.Sepa.MsgId} {_meseAnno}-{_numLiquidazione}";
            grpHdr.CreDtTm = DateTime.Now;
            grpHdr.NbOfTxs = _l.Select(x => x.Iban.ToUpper().RemoveWhiteSpace()).Distinct().Count(); // (byte)_liquidazione.LiquidazionePraticheRegionali.Count();
            grpHdr.CtrlSum = (decimal)_liquidazione.LiquidazionePraticheRegionali.Sum(x => x.PraticheRegionaliImprese.ImportoContributoNetto).GetValueOrDefault();

            payment.GrpHdr = grpHdr;

            //InitgPty
            CBIPaymentRequestGrpHdrInitgPty initgPty = new CBIPaymentRequestGrpHdrInitgPty();
            initgPty.Nm = _config.Sepa.InitgPty_Nm;

            CBIPaymentRequestGrpHdrInitgPtyID id = new CBIPaymentRequestGrpHdrInitgPtyID();
            CBIPaymentRequestGrpHdrInitgPtyIDOrgId orgId = new CBIPaymentRequestGrpHdrInitgPtyIDOrgId();
            CBIPaymentRequestGrpHdrInitgPtyIDOrgIdOthr othr = new CBIPaymentRequestGrpHdrInitgPtyIDOrgIdOthr();
            othr.Issr = _config.Sepa.InitgPty_OrgId_Issr;
            othr.Id = _config.Sepa.InitgPty_OrgId_Id;

            orgId.Othr = othr;
            id.OrgId = orgId;
            initgPty.Id = id;

            payment.GrpHdr.InitgPty = initgPty;

            //FwdgAgt
            CBIPaymentRequestGrpHdrFwdgAgt fwdgAgt = new CBIPaymentRequestGrpHdrFwdgAgt();
            CBIPaymentRequestGrpHdrFwdgAgtFinInstnId finInstnId = new CBIPaymentRequestGrpHdrFwdgAgtFinInstnId();
            CBIPaymentRequestGrpHdrFwdgAgtFinInstnIdClrSysMmbId clrSysMmbId = new CBIPaymentRequestGrpHdrFwdgAgtFinInstnIdClrSysMmbId();
            clrSysMmbId.MmbId = _config.Sepa.ClrSysMmbId_MmbId;

            finInstnId.ClrSysMmbId = clrSysMmbId;
            fwdgAgt.FinInstnId = finInstnId;

            payment.GrpHdr.FwdgAgt = fwdgAgt;

            //PmtInf
            CBIPaymentRequestPmtInf pmtInf = new CBIPaymentRequestPmtInf();
            pmtInf.PmtInfId = $"{_config.Sepa.PmtInf_PmtInfId} {_meseAnno}";
            pmtInf.PmtMtd = _config.Sepa.PmtInf_PmtMtd;
            pmtInf.ChrgBr = _config.Sepa.PmtInf_ChrgBr;
            pmtInf.ReqdExctnDt = DateTime.Now.Date;

            CBIPaymentRequestPmtInfPmtTpInf pmtTpInf = new CBIPaymentRequestPmtInfPmtTpInf();
            CBIPaymentRequestPmtInfPmtTpInfSvcLvl svcLvl = new CBIPaymentRequestPmtInfPmtTpInfSvcLvl();
            svcLvl.Cd = _config.Sepa.PmtInf_PmtTpInf_SvcLvl_Cd;

            pmtTpInf.SvcLvl = svcLvl;

            pmtInf.PmtTpInf = pmtTpInf;

            //Dbtr
            CBIPaymentRequestPmtInfDbtr dbtr = new CBIPaymentRequestPmtInfDbtr();
            dbtr.Nm = _config.Sepa.PmtInf_Dbtr_Nm;

            CBIPaymentRequestPmtInfDbtrPstlAdr pstlAdr = new CBIPaymentRequestPmtInfDbtrPstlAdr();
            pstlAdr.StrtNm = _config.Sepa.PmtInf_Dbtr_PstlAdr_StrtNm;
            pstlAdr.PstCd = _config.Sepa.PmtInf_Dbtr_PstlAdr_PstCd;
            pstlAdr.TwnNm = _config.Sepa.PmtInf_Dbtr_PstlAdr_TwnNm;
            pstlAdr.Ctry = _config.Sepa.PmtInf_Dbtr_PstlAdr_Ctry;

            dbtr.PstlAdr = pstlAdr;

            pmtInf.Dbtr = dbtr;

            //DbtrAcct
            CBIPaymentRequestPmtInfDbtrAcct dbtrAcct = new CBIPaymentRequestPmtInfDbtrAcct();
            CBIPaymentRequestPmtInfDbtrAcctID dbtrAcctID = new CBIPaymentRequestPmtInfDbtrAcctID();
            dbtrAcctID.IBAN = _config.Sepa.PmtInf_DbtrAcct_Iban;

            dbtrAcct.Id = dbtrAcctID;

            pmtInf.DbtrAcct = dbtrAcct;

            //DbtrAgt
            CBIPaymentRequestPmtInfDbtrAgt dbtrAgt = new CBIPaymentRequestPmtInfDbtrAgt();
            CBIPaymentRequestPmtInfDbtrAgtFinInstnId dbtrAgtFinInstnId = new CBIPaymentRequestPmtInfDbtrAgtFinInstnId();
            CBIPaymentRequestPmtInfDbtrAgtFinInstnIdClrSysMmbId dbtrAgtFinInstnIdClrSysMmbId = new CBIPaymentRequestPmtInfDbtrAgtFinInstnIdClrSysMmbId();
            dbtrAgtFinInstnIdClrSysMmbId.MmbId = _config.Sepa.PmtInf_DbtrAgt_FinInstnId_ClrSysMmbId_MmbId;

            dbtrAgtFinInstnId.ClrSysMmbId = dbtrAgtFinInstnIdClrSysMmbId;
            dbtrAgt.FinInstnId = dbtrAgtFinInstnId;

            pmtInf.DbtrAgt = dbtrAgt;


            List<CBIPaymentRequestPmtInfCdtTrfTxInf> listCdtTrfTxInf = new List<CBIPaymentRequestPmtInfCdtTrfTxInf>();

            var i = 0;


            foreach (var item in _l.Select(x=>x.Iban.ToUpper().RemoveWhiteSpace()).Distinct())
            {
                var _riga = _l.FirstOrDefault(x => x.Iban.ToUpper().RemoveWhiteSpace() == item.ToUpper().RemoveWhiteSpace());
                var _somma = _l.Where(x=>x.Iban.ToUpper().RemoveWhiteSpace() == item.ToUpper().RemoveWhiteSpace())?.Sum(x=>x.ImportoContributoNetto).GetValueOrDefault();

                CBIPaymentRequestPmtInfCdtTrfTxInf cdtTrfTxInf = new CBIPaymentRequestPmtInfCdtTrfTxInf();

                CBIPaymentRequestPmtInfCdtTrfTxInfPmtId pmtId = new CBIPaymentRequestPmtInfCdtTrfTxInfPmtId();

                pmtId.InstrId = i;
                pmtId.EndToEndId = $"{_config.Sepa.PmtInf_CdtTrfTxInf_PmtId_EndToEndId} {_meseAnno}-{_numLiquidazione}-1-{i}"; ;

                cdtTrfTxInf.PmtId = pmtId;

                CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInf pmtInfCdtTrfTxInfPmtTpInf = new CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInf();
                CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInfCtgyPurp ctgyPurp = new CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInfCtgyPurp();
                ctgyPurp.Cd = _config.Sepa.PmtInf_CdtTrfTxInf_PmtTpInf_CtgyPurp_Cd;

                pmtInfCdtTrfTxInfPmtTpInf.CtgyPurp = ctgyPurp;

                cdtTrfTxInf.PmtTpInf = pmtInfCdtTrfTxInfPmtTpInf;

                CBIPaymentRequestPmtInfCdtTrfTxInfAmt amt = new CBIPaymentRequestPmtInfCdtTrfTxInfAmt();
                CBIPaymentRequestPmtInfCdtTrfTxInfAmtInstdAmt instdAmt = new CBIPaymentRequestPmtInfCdtTrfTxInfAmtInstdAmt();
                instdAmt.Value = (decimal)_somma;// (decimal)_riga.ImportoContributoNetto.GetValueOrDefault();
                instdAmt.Ccy = "EUR";

                amt.InstdAmt = instdAmt;

                cdtTrfTxInf.Amt = amt;

                var _nominativo = "";

                if (_riga.TipoRichiesta.IsTipoRichiestaDipendente.GetValueOrDefault())
                {
                    _nominativo = $"{_riga.Dipendente?.Cognome} {_riga.Dipendente?.Nome}";
                }
                else
                {
                    _nominativo = _riga.Azienda?.RagioneSociale;
                }

                // _nominativo = _riga.Azienda?.RagioneSociale;

                CBIPaymentRequestPmtInfCdtTrfTxInfCdtr cdtr = new CBIPaymentRequestPmtInfCdtTrfTxInfCdtr();
                cdtr.Nm = _nominativo;

                cdtTrfTxInf.Cdtr = cdtr;

                CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcct cdtrAcct = new CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcct();
                CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcctID cdtrAcctId = new CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcctID();

                cdtrAcctId.IBAN = _riga.Iban.ToUpper().RemoveWhiteSpace();

                cdtrAcct.Id = cdtrAcctId;

                cdtTrfTxInf.CdtrAcct = cdtrAcct;

                CBIPaymentRequestPmtInfCdtTrfTxInfPurp purp = new CBIPaymentRequestPmtInfCdtTrfTxInfPurp();
                purp.Cd = _config.Sepa.PmtInf_CdtTrfTxInf_Purp_Cd;

                cdtTrfTxInf.Purp = purp;

                CBIPaymentRequestPmtInfCdtTrfTxInfRmtInf rmtInf = new CBIPaymentRequestPmtInfCdtTrfTxInfRmtInf();
                rmtInf.Ustrd = $"{_config.Sepa.PmtInf_CdtTrfTxInf_RmtInf_Ustrd} {_meseAnno}-{i}";

                cdtTrfTxInf.RmtInf = rmtInf;

                listCdtTrfTxInf.Add(cdtTrfTxInf);
                i++;
            }

            pmtInf.CdtTrfTxInf = listCdtTrfTxInf.ToArray();

            //alla fine
            payment.PmtInf = pmtInf;

            string r = "";

            using (MemoryStream memS = new MemoryStream())
            {
                //set up the xml settings
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };
                //settings.OmitXmlDeclaration = true;
                Type stype = typeof(CBIPaymentRequest);

                var xmlSerializer = new XmlSerializer(stype);

                using (XmlWriter writer = XmlTextWriter.Create(memS, settings))
                {
                    //write the XML to a stream
                    xmlSerializer.Serialize(writer, payment);
                    writer.Close();
                }

                //encode the memory stream to xml
                //string v = Encoding.UTF8.GetString(memS.ToArray());
                //r = v;

                //memS.Close(); // the using should do this anyway

                return memS;
            }
        }
    }
}

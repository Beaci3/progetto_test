using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Models
{

    // NOTA: con il codice generato potrebbe essere richiesto almeno .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00", IsNullable = false)]
    public partial class CBIPaymentRequest
    {

        private CBIPaymentRequestGrpHdr grpHdrField;

        private CBIPaymentRequestPmtInf pmtInfField;

        /// <remarks/>
        public CBIPaymentRequestGrpHdr GrpHdr
        {
            get
            {
                return this.grpHdrField;
            }
            set
            {
                this.grpHdrField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInf PmtInf
        {
            get
            {
                return this.pmtInfField;
            }
            set
            {
                this.pmtInfField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdr
    {

        private string msgIdField;

        private System.DateTime creDtTmField;

        private int nbOfTxsField;

        private decimal ctrlSumField;

        private CBIPaymentRequestGrpHdrInitgPty initgPtyField;

        private CBIPaymentRequestGrpHdrFwdgAgt fwdgAgtField;

        /// <remarks/>
        public string MsgId
        {
            get
            {
                return this.msgIdField;
            }
            set
            {
                this.msgIdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime CreDtTm
        {
            get
            {
                return this.creDtTmField;
            }
            set
            {
                this.creDtTmField = value;
            }
        }

        /// <remarks/>
        public int NbOfTxs
        {
            get
            {
                return this.nbOfTxsField;
            }
            set
            {
                this.nbOfTxsField = value;
            }
        }

        /// <remarks/>
        public decimal CtrlSum
        {
            get
            {
                return this.ctrlSumField;
            }
            set
            {
                this.ctrlSumField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestGrpHdrInitgPty InitgPty
        {
            get
            {
                return this.initgPtyField;
            }
            set
            {
                this.initgPtyField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestGrpHdrFwdgAgt FwdgAgt
        {
            get
            {
                return this.fwdgAgtField;
            }
            set
            {
                this.fwdgAgtField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrInitgPty
    {

        private string nmField;

        private CBIPaymentRequestGrpHdrInitgPtyID idField;

        /// <remarks/>
        public string Nm
        {
            get
            {
                return this.nmField;
            }
            set
            {
                this.nmField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestGrpHdrInitgPtyID Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrInitgPtyID
    {

        private CBIPaymentRequestGrpHdrInitgPtyIDOrgId orgIdField;

        /// <remarks/>
        public CBIPaymentRequestGrpHdrInitgPtyIDOrgId OrgId
        {
            get
            {
                return this.orgIdField;
            }
            set
            {
                this.orgIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrInitgPtyIDOrgId
    {

        private CBIPaymentRequestGrpHdrInitgPtyIDOrgIdOthr othrField;

        /// <remarks/>
        public CBIPaymentRequestGrpHdrInitgPtyIDOrgIdOthr Othr
        {
            get
            {
                return this.othrField;
            }
            set
            {
                this.othrField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrInitgPtyIDOrgIdOthr
    {

        private string idField;

        private string issrField;

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string Issr
        {
            get
            {
                return this.issrField;
            }
            set
            {
                this.issrField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrFwdgAgt
    {

        private CBIPaymentRequestGrpHdrFwdgAgtFinInstnId finInstnIdField;

        /// <remarks/>
        public CBIPaymentRequestGrpHdrFwdgAgtFinInstnId FinInstnId
        {
            get
            {
                return this.finInstnIdField;
            }
            set
            {
                this.finInstnIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrFwdgAgtFinInstnId
    {

        private CBIPaymentRequestGrpHdrFwdgAgtFinInstnIdClrSysMmbId clrSysMmbIdField;

        /// <remarks/>
        public CBIPaymentRequestGrpHdrFwdgAgtFinInstnIdClrSysMmbId ClrSysMmbId
        {
            get
            {
                return this.clrSysMmbIdField;
            }
            set
            {
                this.clrSysMmbIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestGrpHdrFwdgAgtFinInstnIdClrSysMmbId
    {

        private string mmbIdField;

        /// <remarks/>
        public string MmbId
        {
            get
            {
                return this.mmbIdField;
            }
            set
            {
                this.mmbIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInf
    {

        private string pmtInfIdField;

        private string pmtMtdField;

        private CBIPaymentRequestPmtInfPmtTpInf pmtTpInfField;

        private System.DateTime reqdExctnDtField;

        private CBIPaymentRequestPmtInfDbtr dbtrField;

        private CBIPaymentRequestPmtInfDbtrAcct dbtrAcctField;

        private CBIPaymentRequestPmtInfDbtrAgt dbtrAgtField;

        private string chrgBrField;

        private CBIPaymentRequestPmtInfCdtTrfTxInf[] cdtTrfTxInfField;

        /// <remarks/>
        public string PmtInfId
        {
            get
            {
                return this.pmtInfIdField;
            }
            set
            {
                this.pmtInfIdField = value;
            }
        }

        /// <remarks/>
        public string PmtMtd
        {
            get
            {
                return this.pmtMtdField;
            }
            set
            {
                this.pmtMtdField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfPmtTpInf PmtTpInf
        {
            get
            {
                return this.pmtTpInfField;
            }
            set
            {
                this.pmtTpInfField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ReqdExctnDt
        {
            get
            {
                return this.reqdExctnDtField;
            }
            set
            {
                this.reqdExctnDtField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtr Dbtr
        {
            get
            {
                return this.dbtrField;
            }
            set
            {
                this.dbtrField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrAcct DbtrAcct
        {
            get
            {
                return this.dbtrAcctField;
            }
            set
            {
                this.dbtrAcctField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrAgt DbtrAgt
        {
            get
            {
                return this.dbtrAgtField;
            }
            set
            {
                this.dbtrAgtField = value;
            }
        }

        /// <remarks/>
        public string ChrgBr
        {
            get
            {
                return this.chrgBrField;
            }
            set
            {
                this.chrgBrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CdtTrfTxInf")]
        public CBIPaymentRequestPmtInfCdtTrfTxInf[] CdtTrfTxInf
        {
            get
            {
                return this.cdtTrfTxInfField;
            }
            set
            {
                this.cdtTrfTxInfField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfPmtTpInf
    {

        private CBIPaymentRequestPmtInfPmtTpInfSvcLvl svcLvlField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfPmtTpInfSvcLvl SvcLvl
        {
            get
            {
                return this.svcLvlField;
            }
            set
            {
                this.svcLvlField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfPmtTpInfSvcLvl
    {

        private string cdField;

        /// <remarks/>
        public string Cd
        {
            get
            {
                return this.cdField;
            }
            set
            {
                this.cdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtr
    {

        private string nmField;

        private CBIPaymentRequestPmtInfDbtrPstlAdr pstlAdrField;

        /// <remarks/>
        public string Nm
        {
            get
            {
                return this.nmField;
            }
            set
            {
                this.nmField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrPstlAdr PstlAdr
        {
            get
            {
                return this.pstlAdrField;
            }
            set
            {
                this.pstlAdrField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrPstlAdr
    {

        private string strtNmField;

        private string pstCdField;

        private string twnNmField;

        private string ctryField;

        /// <remarks/>
        public string StrtNm
        {
            get
            {
                return this.strtNmField;
            }
            set
            {
                this.strtNmField = value;
            }
        }

        /// <remarks/>
        public string PstCd
        {
            get
            {
                return this.pstCdField;
            }
            set
            {
                this.pstCdField = value;
            }
        }

        /// <remarks/>
        public string TwnNm
        {
            get
            {
                return this.twnNmField;
            }
            set
            {
                this.twnNmField = value;
            }
        }

        /// <remarks/>
        public string Ctry
        {
            get
            {
                return this.ctryField;
            }
            set
            {
                this.ctryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrAcct
    {

        private CBIPaymentRequestPmtInfDbtrAcctID idField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrAcctID Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrAcctID
    {

        private string iBANField;

        /// <remarks/>
        public string IBAN
        {
            get
            {
                return this.iBANField;
            }
            set
            {
                this.iBANField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrAgt
    {

        private CBIPaymentRequestPmtInfDbtrAgtFinInstnId finInstnIdField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrAgtFinInstnId FinInstnId
        {
            get
            {
                return this.finInstnIdField;
            }
            set
            {
                this.finInstnIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrAgtFinInstnId
    {

        private CBIPaymentRequestPmtInfDbtrAgtFinInstnIdClrSysMmbId clrSysMmbIdField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfDbtrAgtFinInstnIdClrSysMmbId ClrSysMmbId
        {
            get
            {
                return this.clrSysMmbIdField;
            }
            set
            {
                this.clrSysMmbIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfDbtrAgtFinInstnIdClrSysMmbId
    {

        private string mmbIdField;

        /// <remarks/>
        public string MmbId
        {
            get
            {
                return this.mmbIdField;
            }
            set
            {
                this.mmbIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInf
    {

        private CBIPaymentRequestPmtInfCdtTrfTxInfPmtId pmtIdField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInf pmtTpInfField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfAmt amtField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfCdtr cdtrField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcct cdtrAcctField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfPurp purpField;

        private CBIPaymentRequestPmtInfCdtTrfTxInfRmtInf rmtInfField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfPmtId PmtId
        {
            get
            {
                return this.pmtIdField;
            }
            set
            {
                this.pmtIdField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInf PmtTpInf
        {
            get
            {
                return this.pmtTpInfField;
            }
            set
            {
                this.pmtTpInfField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfAmt Amt
        {
            get
            {
                return this.amtField;
            }
            set
            {
                this.amtField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfCdtr Cdtr
        {
            get
            {
                return this.cdtrField;
            }
            set
            {
                this.cdtrField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcct CdtrAcct
        {
            get
            {
                return this.cdtrAcctField;
            }
            set
            {
                this.cdtrAcctField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfPurp Purp
        {
            get
            {
                return this.purpField;
            }
            set
            {
                this.purpField = value;
            }
        }

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfRmtInf RmtInf
        {
            get
            {
                return this.rmtInfField;
            }
            set
            {
                this.rmtInfField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfPmtId
    {

        private int instrIdField;

        private string endToEndIdField;

        /// <remarks/>
        public int InstrId
        {
            get
            {
                return this.instrIdField;
            }
            set
            {
                this.instrIdField = value;
            }
        }

        /// <remarks/>
        public string EndToEndId
        {
            get
            {
                return this.endToEndIdField;
            }
            set
            {
                this.endToEndIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInf
    {

        private CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInfCtgyPurp ctgyPurpField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInfCtgyPurp CtgyPurp
        {
            get
            {
                return this.ctgyPurpField;
            }
            set
            {
                this.ctgyPurpField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfPmtTpInfCtgyPurp
    {

        private string cdField;

        /// <remarks/>
        public string Cd
        {
            get
            {
                return this.cdField;
            }
            set
            {
                this.cdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfAmt
    {

        private CBIPaymentRequestPmtInfCdtTrfTxInfAmtInstdAmt instdAmtField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfAmtInstdAmt InstdAmt
        {
            get
            {
                return this.instdAmtField;
            }
            set
            {
                this.instdAmtField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfAmtInstdAmt
    {

        private string ccyField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Ccy
        {
            get
            {
                return this.ccyField;
            }
            set
            {
                this.ccyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfCdtr
    {

        private string nmField;

        /// <remarks/>
        public string Nm
        {
            get
            {
                return this.nmField;
            }
            set
            {
                this.nmField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcct
    {

        private CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcctID idField;

        /// <remarks/>
        public CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcctID Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfCdtrAcctID
    {

        private string iBANField;

        /// <remarks/>
        public string IBAN
        {
            get
            {
                return this.iBANField;
            }
            set
            {
                this.iBANField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfPurp
    {

        private string cdField;

        /// <remarks/>
        public string Cd
        {
            get
            {
                return this.cdField;
            }
            set
            {
                this.cdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:CBI:xsd:CBIPaymentRequest.00.04.00")]
    public partial class CBIPaymentRequestPmtInfCdtTrfTxInfRmtInf
    {

        private string ustrdField;

        /// <remarks/>
        public string Ustrd
        {
            get
            {
                return this.ustrdField;
            }
            set
            {
                this.ustrdField = value;
            }
        }
    }



}

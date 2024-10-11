﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EBLIG.DOM.Models
//{
//    public class UniemensModel
//    {
//        public _Id _id { get; set; }
//        public int? id_ebt { get; set; }
//        public int? id_azienda { get; set; }
//        public int? id_centro_servizi { get; set; }
//        public string ragione_sociale { get; set; }
//        public string matricola_inps { get; set; }
//        public string codice_fiscale { get; set; }
//        public string partita_iva { get; set; }
//        public string csc { get; set; }
//        public Tipo tipo { get; set; }
//        public string comune { get; set; }
//        public Mensilita[] mensilita { get; set; }
//        public Totali totali { get; set; }
//        public Data_Update data_update { get; set; }
//    }

//    public class _Id
//    {
//        public string oid { get; set; }
//    }

//    public class Tipo
//    {
//        public string ikey { get; set; }
//        public string ivalue { get; set; }
//    }

//    public class Totali
//    {
//        public float? entrate { get; set; }
//        public float? movimenti { get; set; }
//        public Dovuti[] dovuti { get; set; }
//    }

//    public class Dovuti
//    {
//        public int? id_quota { get; set; }
//        public string quota { get; set; }
//        public int? ordine { get; set; }
//        public float? importo { get; set; }
//    }

//    public class Data_Update
//    {
//        public DateTime? date { get; set; }
//    }

//    public class Mensilita
//    {
//        public string mese { get; set; }
//        public Entrate[] entrate { get; set; }
//        public object[] movimenti { get; set; }
//        public Dovuti2[] dovuti { get; set; }
//        public Totali1 totali { get; set; }
//    }

//    public class Totali1
//    {
//        public float? entrate { get; set; }
//        public float? movimenti { get; set; }
//        public Dovuti1[] dovuti { get; set; }
//    }

//    public class Dovuti1
//    {
//        public int? id_quota { get; set; }
//        public string quota { get; set; }
//        public int? ordine { get; set; }
//        public float? importo { get; set; }
//    }

//    public class Entrate
//    {
//        public Id_Entrata id_entrata { get; set; }
//        public string descrizione { get; set; }
//        public float? importo { get; set; }
//        public Metodo_Pagamento metodo_pagamento { get; set; }
//        public Causale causale { get; set; }
//        public Tipo_Pagamento tipo_pagamento { get; set; }
//        public Rel_Source rel_source { get; set; }
//    }

//    public class Id_Entrata
//    {
//        public string oid { get; set; }
//    }

//    public class Metodo_Pagamento
//    {
//        public string ikey { get; set; }
//        public string ivalue { get; set; }
//    }

//    public class Causale
//    {
//        public string ikey { get; set; }
//        public string ivalue { get; set; }
//    }

//    public class Tipo_Pagamento
//    {
//        public string ikey { get; set; }
//        public string ivalue { get; set; }
//    }

//    public class Rel_Source
//    {
//        public string id_f24 { get; set; }
//        public object id_f24_anticipato { get; set; }
//        public Id_Flusso id_flusso { get; set; }
//        public Date_Update date_update { get; set; }
//        public string user_update { get; set; }
//    }

//    public class Id_Flusso
//    {
//        public string oid { get; set; }
//    }

//    public class Date_Update
//    {
//        public DateTime? date { get; set; }
//    }

//    public class Dovuti2
//    {
//        public Id_Dovuto id_dovuto { get; set; }
//        public int? id_dipendente { get; set; }
//        public int? id_iscritto { get; set; }
//        public string codice_fiscale { get; set; }
//        public string nome { get; set; }
//        public string cognome { get; set; }
//        public string codice_contratto { get; set; }
//        public string qualifica1 { get; set; }
//        public string qualifica2 { get; set; }
//        public string qualifica3 { get; set; }
//        public Causale1 causale { get; set; }
//        public float? imponibile { get; set; }
//        public Quote[] quote { get; set; }
//        public Rel_Source1 rel_source { get; set; }
//        public Data_Update1 data_update { get; set; }
//    }

//    public class Id_Dovuto
//    {
//        public string oid { get; set; }
//    }

//    public class Causale1
//    {
//        public string ikey { get; set; }
//        public string ivalue { get; set; }
//    }

//    public class Rel_Source1
//    {
//        public string id_uniemens { get; set; }
//        public Id_Flusso1 id_flusso { get; set; }
//        public Date_Update1 date_update { get; set; }
//        public string user_update { get; set; }
//    }

//    public class Id_Flusso1
//    {
//        public string oid { get; set; }
//    }

//    public class Date_Update1
//    {
//        public DateTime? date { get; set; }
//    }

//    public class Data_Update1
//    {
//        public DateTime? date { get; set; }
//    }

//    public class Quote
//    {
//        public int? id_quota { get; set; }
//        public string quota { get; set; }
//        public int? ordine { get; set; }
//        public DateTime? data_rendicontazione { get; set; }
//        public object data_copertura { get; set; }
//        public float? importo { get; set; }
//    }

//}

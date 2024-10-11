
using EBLIG.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Data
{
    public class EBLIGDbContext : DbContext
    {
        //public EBLIGDbContext() : base("data Source=.\\;Initial Catalog=EBLIG;Integrated Security=True")
        public EBLIGDbContext() : base("EBLIGDbContext")
        {
            Database.SetInitializer<EBLIGDbContext>(null);
        }

        public DbSet<ConsulenteCS> ConsulenteCS { get; set; }

        public DbSet<Azienda> Azienda { get; set; }

        //  Gestione Tabelle >> Metropoliotane <<
        public DbSet<Regioni> Regioni { get; set; }

        public DbSet<Province> Province { get; set; }

        public DbSet<Comuni> Comuni { get; set; }

        public DbSet<Localita> Localita { get; set; }
        public DbSet<Motivazioni> Motivazioni { get; set; }
        //  Fine elenco

        public DbSet<Tipologia> Tipologia { get; set; }

        public DbSet<Allegati> Allegati { get; set; }

        public DbSet<TipoRichiestaAllegati> TipoRichiestaAllegati { get; set; }

        public DbSet<PraticheRegionaliImprese> PraticheRegionaliImprese { get; set; }

        public DbSet<PraticheRegionaliImpreseDatiPratica> PraticheRegionaliImpreseDatiPratica { get; set; }

        public DbSet<PraticheRegionaliImpreseAllegati> PraticheRegionaliImpreseAllegati { get; set; }

        public DbSet<StatoPratica> StatoPratica { get; set; }

        public DbSet<TipoRichiesta> TipoRichiesta { get; set; }

        public DbSet<Azioni> Azioni { get; set; }

        public DbSet<AzioniRuolo> AzioniRuolo { get; set; }

        public DbSet<MotivazioniRichiesta> MotivazioniRichiesta { get; set; }

        public DbSet<DelegheConsulenteCSAzienda> DelegheConsulenteCSAzienda { get; set; }

        public DbSet<Dipendente> Dipendente { get; set; }
       
        public DbSet<DipendenteAzienda> DipendenteAzienda { get; set; }
        
        public DbSet<TempoLavoro> TempoLavoro { get; set; }
        
        public DbSet<TipoContratto> TipoContratto { get; set; }
        
        public DbSet<TipoImpiego> TipoImpiego { get; set; }
        
        public DbSet<Parentela> Parentela { get; set; }
        
        public DbSet<Liquidazione> Liquidazione { get; set; }
        
        public DbSet<LiquidazionePraticheRegionali> LiquidazionePraticheRegionali { get; set; }
        
        public DbSet<StatoLiquidazione> StatoLiquidazione { get; set; }

        public DbSet<Uniemens> Uniemens { get; set; }

        public DbSet<Logs> Logs { get; set; }

        public DbSet<Copertura> Copertura { get; set; }

        public DbSet<Sportello> Sportello{ get; set; }

        public DbSet<DelegheSportelloDipendente> DelegheSportelloDipendente { get; set; }

        public DbSet<NavigatioHistory> NavigatioHistory { get; set; }

        public DbSet<AvvisoUtente> AvvisoUtente { get; set; }

        public DbSet<AvvisoUtenteRuoli> AvvisoUtenteRuoli { get; set; }

        public DbSet<LiquidazionePraticheRegionaliMailInviatiEsito> LiquidazionePraticheRegionaliMailInviatiEsito { get; set; }

        public DbSet<Utente> Utenti { get; set; }
    }
}

using EBLIG.DOM.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOMTests
{
    internal class TruncateTables
    {
        [TestMethod]
        public void Truncate()
        {
            EBLIGDbContext dbcontext  = new EBLIGDbContext();
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE PraticheRegionaliImpreseStatoPraticaStorico");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE PraticheRegionaliImpreseDatiPratica");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE PraticheRegionaliImpreseAllegati");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE PraticheRegionaliImprese");

            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE LiquidazionePraticheRegionali");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE Liquidazione");

            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE DelegheConsulenteCSAzienda");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE DipendenteAzienda");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE Azienda");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE ConsulenteCS");
            dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE Dipendente");
        }
    }
}

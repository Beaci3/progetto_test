using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBLIG.DOM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.DAL.Tests
{
    [TestClass()]
    public class PraticheAziendaUtilityTests
    {
        [TestMethod()]
        public void GetImportoTotaleRimborsatoSicurezzaLavoroImpreseTest()
        {
            var x = PraticheAziendaUtility.GetImportoTotaleRimborsatoSicurezzaLavoroImprese(10039.50m);
        }
    }
}
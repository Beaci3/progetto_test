using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBLIG.DOM.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.DOM.Providers.Tests
{
    [TestClass()]
    public class LiquidazioneIdProviderTests
    {
        [TestMethod()]
        public void ProcessSendMailLiquidazioneTest()
        {
            LiquidazioneIdProvider p = new LiquidazioneIdProvider();
            p.OnSendMailLiquidazioneReport += P_OnSendMailLiquidazioneReport;
            p.ProcessSendMailLiquidazione(5);
        }

        private string P_OnSendMailLiquidazioneReport(LiquidazioneIdProvider.SendMailLiquidazioneEmailResultModel model)
        {
           return "ok";
        }
    }
}
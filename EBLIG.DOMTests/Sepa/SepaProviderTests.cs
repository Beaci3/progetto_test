using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBLIG.DOM.Sepa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBLIG.DOM.Providers;

namespace EBLIG.DOM.Sepa.Tests
{
    [TestClass()]
    public class SepaProviderTests
    {
        [TestMethod()]
        public void CreateSepaTest()
        {
            SepaProvider P = new SepaProvider();
          //var xml=  P.SepaXmlString(20);
          var base64=  P.SepaBase64String(20);
        }
    }
}
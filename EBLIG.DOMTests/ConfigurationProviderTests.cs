using Microsoft.VisualStudio.TestTools.UnitTesting;
using EBLIG.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBLIG.Utils;

namespace EBLIG.DOM.Tests
{
    [TestClass()]
    public class ConfigurationProviderTests
    {
        [TestMethod()]
        public void SaveConfiguration()
        {
            ConfigurationProvider p = new ConfigurationProvider();
            p.SaveConfiguration();
        }
    }
}
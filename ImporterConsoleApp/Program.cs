using EBLIG.DOM.Importer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ImportProvider p = new ImportProvider();

            //p.OnReport += P_OnReport;
            //p.ProcessAiende();
        }

        private static void P_OnReport(string id, string username, int index, int totale, string message)
        {
            Console.WriteLine(message);
        }
    }
}

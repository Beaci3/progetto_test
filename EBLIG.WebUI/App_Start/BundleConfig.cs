using System.Web;
using System.Web.Optimization;

namespace EBLIG.WebUI
{
    public class BundleConfig
    {
        // Per altre informazioni sulla creazione di bundle, vedere https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/inputs.js",
                        "~/Scripts/requiredSpan.js",
                        "~/Scripts/checkboxValidation.js",
                        "~/Scripts/validateHidden.js",
                        "~/Scripts/jquery.validate.methods.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js",
                      "~/Scripts/jquery.matchHeight.js",
                      "~/Scripts/jquery.mask.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                        "~/Scripts/globalize/globalize.js",
                        "~/Scripts/globalize/cultures/globalize.cultures.js",
                        "~/Scripts/globalize/cultures/globalize.culture.it-IT.js"));

            bundles.Add(new ScriptBundle("~/bundles/utils").Include(
                      "~/Scripts/utility.js",
                      "~/Scripts/sweetalert2.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/modale.js"));

            // Utilizzare la versione di sviluppo di Modernizr per eseguire attività di sviluppo e formazione. Successivamente, quando si è
            // pronti per passare alla produzione, usare lo strumento di compilazione disponibile all'indirizzo https://modernizr.com per selezionare solo i test necessari.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     // "~/Content/bootstrap.css",
                      "~/Content/bootstrap.cosmo.min.css",
                      "~/Content/bootstrap.custom.css",
                      "~/Content/site.css",
                      "~/Content/toastr.css",
                      "~/Content/modale.css"));
        }
    }
}

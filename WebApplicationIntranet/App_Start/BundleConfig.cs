using System.Web;
using System.Web.Optimization;

namespace WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                "~/Scripts/chart/highcharts.js"));
            bundles.Add(new ScriptBundle("~/bundles/tools").Include(
                "~/Scripts/function.tools/fast-functions.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jqueryMaxLength.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/chosen.jquery.js", "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/notifications").Include(
                        "~/Scripts/SmartNotification.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/boostrapdatetimepicker").Include(
                "~/Scripts/moment-with-locales.min.js",
                "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/mask").Include(
               // "~/Scripts/jquery.inputmask/jquery.inputmask.date.extensions.js",
                //"~/Scripts/jquery.inputmask/jquery.inputmask.extensions.js",
                "~/Scripts/jquery.inputmask/jquery.inputmask.js",
                "~/Scripts/jquery.inputmask/jquery.inputmask.numeric.extensions.js"//,
               // "~/Scripts/jquery.inputmask/jquery.inputmask.phone.extensions.js",
                /*"~/Scripts/jquery.inputmask/jquery.inputmask.regex.extensions.js"*/));

            bundles.Add(new ScriptBundle("~/bundles/customscripts").Include(
                        "~/Scripts/Scripts.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/journal.css",
                      "~/Content/site.css",
                      "~/Content/smartadmin-production.min.css",
                      "~/Content/smartadmin-rtl.min.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/PagedList.css",
                      "~/Content/chosen.css",
                      "~/Content/font-awesome.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}

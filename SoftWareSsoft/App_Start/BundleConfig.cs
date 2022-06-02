using System.Web;
using System.Web.Optimization;

namespace SoftWareSsoft
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquerymin").Include(
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/BootstrapCss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/MainCss").Include(
                    "~/Content/font-awesome.css",
                   "~/Content/css/Style.css",
                   "~/Content/css/responsive.css"));
            //---- Admin
            bundles.Add(new ScriptBundle("~/bundles/Login").Include(
                  "~/Scripts/vue.min.js",
                 "~/Assets/vendors/js/moment.min.js",
                 "~/Scripts/Main.js",
                  "~/Assets/Admin/Login.js"));
            bundles.Add(new ScriptBundle("~/bundles/AdminJs").Include(
                    "~/Scripts/vue.min.js",
                   "~/Assets/vendors/js/bootstrap-progressbar.min.js",
                   "~/Assets/vendors/js/moment.min.js",
                   "~/Content/pnotify/pnotify.js",
                   "~/Assets/vendors/js/jquery.mCustomScrollbar.concat.min.js",
                   "~/Scripts/Main.js",
                   "~/Assets/vendors/js/custom.min.js"));
            bundles.Add(new StyleBundle("~/Content/AdminCss").Include(
                 "~/Content/font-awesome.css",
                 "~/Assets/vendors/css/jquery.mCustomScrollbar.min.css",
                 "~/Content/pnotify/pnotify.css",
                 "~/Assets/vendors/css/custom.css",
                 "~/Assets/vendors/css/Response.css"));

            // --- Theme SSoft
            bundles.Add(new StyleBundle("~/Content/Ssoft").Include(
                 "~/Content/css/Theme/SsoftVn.css",
                   "~/Content/css/Theme/Ssoftresponsive.css"));
            bundles.Add(new ScriptBundle("~/bundles/Mainjs").Include(
                    "~/Scripts/vue.min.js",
                    "~/Scripts/moment.min.js",
                     "~/Content/inputmask/jquery.inputmask.bundle.js",
                    "~/Scripts/Main.js",
                    "~/Scripts/Theme/SsoftMain.js"));
        }
    }
}
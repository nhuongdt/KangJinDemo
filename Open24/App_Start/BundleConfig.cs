using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Open24 
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/staticjs").Include(
                       "~/Scripts/jquery-{version}.js",
                           "~/Scripts/knockout-{version}.js",
                        "~/Assets/js/Client/Main.js",
                        "~/Content/js/moment.js"));
            bundles.Add(new ScriptBundle("~/bundles/public").Include(
                      "~/Scripts/vue.min.js",
                      "~/Assets/js/Client/Main.js",
                      "~/Content/js/moment.js",
                      "~/Assets/js/Client/open24/open24.js",
                      "~/Assets/js/Client/open24/localValidateFunction.js"
                      ));

            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*",
                        "~/Content/js/ConvertVie.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Content/js/main.js"));

         
            bundles.Add(new StyleBundle("~/bundles/LayoutClient").Include(
                    "~/Content/css/imo.css",
                    "~/Content/bootstrap.css",
                     "~/Content/font-awesome.css",
                     "~/Content/css/jquery.bxslider.css",
                    "~/Content/css/style.css",
                    "~/Content/css/responsive.css"
                     ));
            bundles.Add(new StyleBundle("~/bundles/LayoutOpen24").Include(
                    "~/Content/css/imo.css",
                    "~/Content/bootstrap.css",
                    "~/Content/font-awesome.css",
                    "~/Content/css/jquery.bxslider.css",
                    "~/Content/css/animations.css",
                    "~/Content/css/StyleOpen24.css",
                    "~/Content/css/responsiveOpen24.css",
                    "~/Content/css/NewCommonUI.css",
                    "~/Content/css/NewUI.css"
                    
                    ));
            bundles.Add(new ScriptBundle("~/bundles/abc").Include(
                      "~/Scripts/knockout-{version}.js",
                    "~/Scripts/respond.js",
                    "~/Content/js/bootstrap.min.js",
                    "~/Scripts/Post/Post.js",
                    "~/Scripts/jquery.tablesorter.pager.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/HomeIndex").Include(
                   "~/Content/js/jsslide.js",
                    "~/Assets/css/Slide/jquery-gallery.js",
                     "~/Assets/js/Client/Home.js"
                   ));
            bundles.Add(new ScriptBundle("~/bundles/client").Include(
                    "~/Scripts/Post/Client.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                      "~/Scripts/knockout-{version}.js",
                    "~/Scripts/Post/Home.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/nhombaiviet").Include(
                      "~/Scripts/knockout-{version}.js",
                    "~/Scripts/Post/nhombaiviet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                    //"~/Scripts/jquery-1.12.4.js",
                    "~/Scripts/jquery.dataTables.min.js",
                    "~/Scripts/dataTables.bootstrap.min.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/layoutFontend").Include(
                  "~/Scripts/modernizr-*",
                    "~/Content/js/ConvertVie.js",
                   "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js",
                    "~/Content/js/main.js",
                   "~/Assets/js/Client/AppChat.js"
                   ));
            bundles.Add(new ScriptBundle("~/bundles/Knockout").Include(
                   "~/Scripts/knockout-{version}.js",
                 "~/Assets/js/Admin/Main.js",
                 "~/Content/js/jquery.growl.js",
                 "~/Content/js/moment.js",
                 "~/Content/js/bootstrap-datetimepicker.min.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/HomeClient").Include(
                "~/Content/js/moment.js",
               "~/Assets/js/Client/Home.js",
                "~/Content/js/jsslide.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/ThanhToan").Include(
                     "~/Scripts/jquery-{version}.js",
                      "~/Scripts/knockout-{version}.js",
                     "~/Assets/js/Client/Main.js",
                     "~/Assets/js/Client/payents.js"
             ));
            bundles.Add(new ScriptBundle("~/bundles/Js").Include(
              "~/Content/js/jquery-3.2.1.min.js",
              "~/Content/bootstrap-5.1.3-dist/js/bootstrap.js",
              "~/Scripts/respond.js",
              "~/Content/js/moment.js",
              "~/Scripts/knockout-3.4.2.js",
               "~/Scripts/vue.min.js"
                 ));
            bundles.Add(new ScriptBundle("~/bundles/Open24Js").Include(

                      "~/Assets/js/Client/Main.js",
                      "~/Assets/js/Client/open24/open24.js",
                      "~/Assets/js/Client/open24/localValidateFunction.js"
                  ));

            bundles.Add(new StyleBundle("~/bundles/Css").Include(
                    "~/Content/css/imo.css",
                    "~/Content/bootstrap-5.1.3-dist/css/bootstrap.css",
                    "~/Content/font-awesome.css",
                    "~/Content/css/jquery.bxslider.css",
                    "~/Content/css/animations.css"
                    ));

            bundles.Add(new StyleBundle("~/bundles/Open24Css").Include(

                  //"~/Content/css/responsive.css",

                  //"~/Content/css/style.css",
                  "~/Content/css/StyleOpen24.css",
                  "~/Content/css/responsiveOpen24.css",
                  "~/Content/css/NewCommonUI.css",
                  "~/Content/css/NewUI.css"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/OpenAdminJs").Include(
                "~/Scripts/vue.min.js",
                "~/Assets/js/Client/Main.js",
                "~/Assets/js/Client/open24/localValidateFunction.js"
            ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
using System.Web;
using System.Web.Optimization;

namespace MVCExport
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                       "~/App_JS/compounents/jquery/2.1.1/jquery.js",
                       "~/App_JS/compounents/bootstrap/3.1.1/js/bootstrap.js",
                       "~/App_JS/compounents/angular/1.3.0/angular.js",
                       "~/App_JS/compounents/angular/1.3.0/angular-route.js",
                       "~/App_JS/compounents/angular/1.3.0/angular-sanitize.js",
                       "~/App_JS/compounents/angular/1.3.0/angular-animate.js",
                       "~/App_JS/ng-app/app.js",
                       "~/App_JS/ng-app/config.js",
                       "~/App_JS/ng-app/factories/logger.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/scriptsLegacybrowser").Include(
                       "~/App_JS/compounents/jquery/1.11.1/jquery.js",
                       "~/App_JS/compounents/bootstrap/3.1.1/js/bootstrap.js",
                       "~/App_JS/compounents/angular/1.2.9/angular.js",
                       "~/App_JS/compounents/angular/1.2.9/angular-route.js",
                       "~/App_JS/compounents/angular/1.2.9/angular-sanitize.js",
                       "~/App_JS/ng-app/app.js",
                       "~/App_JS/ng-app/config.js",
                       "~/App_JS/ng-app/factories/logger.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/ieshim").Include(
                    "~/App_JS/compounents/html5shiv/3.7.0/html5shiv.js",
                    "~/App_JS/compounents/respond/respond.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/App_JS/compounents/bootstrap/3.1.1/css/bootstrap.css",
                     "~/App_JS/compounents/font-awesome/4.0.3/css/font-awesome.css",
                     "~/Content/Site.css"
                     ));

            BundleTable.EnableOptimizations = false;
        }
    }
}

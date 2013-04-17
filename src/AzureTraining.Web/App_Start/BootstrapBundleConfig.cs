using System;
using System.Linq;
using System.Web.Optimization;

namespace AzureTraining.Web.App_Start
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.validate.js",
                "~/scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js",
                "~/Scripts/bootstrap-fileupload.min.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/knockout-2.0.0.js"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/body.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap-mvc-validation.css",
                "~/Content/bootstrap-fileupload.min.css",
                "~/Content/datepicker.css",
                "~/Content/Site.css"
                ));
        }
    }
}
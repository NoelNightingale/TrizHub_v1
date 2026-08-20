#region Usings

using System.Web.Optimization;

#endregion Usings

namespace TRiZHub
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/fontAwesome")
                .Include("~/Content/font-awesome.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/ion")
                .Include("~/Content/ion/css/ionicons.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/materialDesign")
                .Include("~/Content/material-design-icons.css"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap/dist/bootstrap.css",
                    "~/Content/animate/animate.min.css",
                    "~/Content/styles/font.css",
                    "~/Content/styles/app.css",
                    "~/Content/loading-bar.css",
                    "~/Content/autocomplete.css",
                    "~/Content/bootstrap-additions.css",
                    //"~/Content/summernote/summernote.css",
                    "~/Content/summernote/summernote-bs4.css",
                    //"~/Content/summernote/summernote-lite.css",
                    "~/Content/trix/trix.css",

                    "~/Content/bootstrap-additions.css",
                    "~/Content/angular-toggle-switch-bootstrap-3.css",
                    "~/Content/angular-slider.css",
                    "~/Content/Site.css",
                    "~/Scripts/angular-ui/select.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/AngularApp")
                .Include("~/Scripts/lib/tether.min.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/jquery-2.2.4.min.js")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-route.js")
                .Include("~/Scripts/angular-ui/ui-bootstrap-tpls.js")
                .Include("~/Scripts/angular-animate.js")
                .Include("~/Scripts/angular-sanitize.js")
                .Include("~/Scripts/lib/angular-ui-router.js")
                .Include("~/Scripts/angular-ui/select.min.js")
                );

            var libBundle = new ScriptBundle("~/bundles/lib")
                .Include("~/Scripts/lib/angular-input-match.js")
                .Include("~/Scripts/lib/showErrors.js")
                .Include("~/Scripts/lib/lodash.js")
                .Include("~/Scripts/lib/loading-bar.js")
                .Include("~/Scripts/lib/ngStorage.min.js")
                .Include("~/Scripts/lib/ocLazyLoad/ocLazyLoad.js")
                .Include("~/Scripts/lib/tcrGrid.js")
                .Include("~/Scripts/lib/tcrPagedData.js")
                .Include("~/Scripts/lib/angular-simple-logger.min.js")
                .Include("~/Scripts/lib/autocomplete.js")
                .Include("~/Scripts/lib/angular-google-maps.js")
                .Include("~/Scripts/lib/moment.js")
                .Include("~/Scripts/lib/summernote/summernote-bs4.js")
                .Include("~/Scripts/lib/summernote/angular-summernote.min.js")

                //.Include("~/Scripts/tinymce/tinymce.min.js")
                //.Include("~/Scripts/ui-tinymce.min.js")

                .Include("~/Scripts/lib/trix/angular-trix.min.js")
                .Include("~/Scripts/lib/angular-touch.js")
                .Include("~/Scripts/lib/angular-slider.min.js")
                .Include("~/Scripts/lib/angular-toggle-switch.min.js");

            libBundle.Transforms.Clear();
            bundles.Add(libBundle);

            var portalBundle = new ScriptBundle("~/bundles/Portal")
                .Include("~/Portals/app/AngularApp.js")
                .Include("~/Scripts/lib/angularstrap.js")
                .Include("~/Scripts/lib/angular-strap.tpl.js")
                .Include("~/Portals/app/app.ctrl.js")
                .Include("~/Portals/app/configuration.js")
                .Include("~/Portals/app/directives/ui-include.js")
                .Include("~/Portals/app/directives/ui-jp.js")
                .IncludeDirectory("~/Portals/app/base", "*.js")
                .IncludeDirectory("~/Portals/app/globals", "*.js")
                .IncludeDirectory("~/Portals/app/services", "*.js")
                .IncludeDirectory("~/Portals/app/services/AccountService", "*.js")
                .IncludeDirectory("~/Portals/app/services/EnumService", "*.js")
                .IncludeDirectory("~/Portals/app/services/MasterDataService", "*.js")
                .IncludeDirectory("~/Portals/app/services/RoleService", "*.js")
                .IncludeDirectory("~/Portals/app/services/SecurityService", "*.js")
                .IncludeDirectory("~/Portals/app/services/UserService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ClientService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ProjectService", "*.js")
                .IncludeDirectory("~/Portals/app/services/TimesheetService", "*.js")
                .IncludeDirectory("~/Portals/app/services/TimesheetTemplateService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ActivityService", "*.js")
                .IncludeDirectory("~/Portals/app/services/TeamService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ReportService", "*.js")
                .IncludeDirectory("~/Portals/app/services/BillingCycleService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ScorecardTemplateService", "*.js")
                .IncludeDirectory("~/Portals/app/services/BillingRatesService", "*.js")
                .IncludeDirectory("~/Portals/app/services/ScorecardService", "*.js")
                .IncludeDirectory("~/Portals/app/services/TravelInformationService", "*.js")
                .IncludeDirectory("~/Portals/app/services/EmployerService", "*.js")
                .IncludeDirectory("~/Portals/app/directives", "*.js");
            bundles.Add(portalBundle);

            BundleTable.EnableOptimizations = false; /*!Debugger.IsAttached;*/
        }
    }
}
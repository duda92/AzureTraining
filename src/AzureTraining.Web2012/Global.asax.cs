using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AzureTraining.Core.WindowsAzure.Helpers;
using AzureTraining.Web.App_Start;
using AzureTraining.Web.Binders;
using AzureTraining.Web.Models;
using Microsoft.WindowsAzure;
using System.Configuration;
using AzureTraining.Core;

namespace AzureTraining.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ApplyCloudConfigs();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BootstrapBundleConfig.RegisterBundles(BundleTable.Bundles);
            
            Bootstrapper.Initialise();

            ModelBinders.Binders.Add(typeof(DocumentUploadViewModel), new DocumentUploadViewModelModelBinder());
            
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(ConfigurationManager.ConnectionStrings[configName].ConnectionString);
            });
        }
  
        private void ApplyCloudConfigs()
        {
            CloudConfigurationHelper.MoveConnectionStringsToConfig(SettingsKeys.DefaultConnection);
            CloudConfigurationHelper.MoveConnectionStringsToConfig(SettingsKeys.DataConnectionString);
            CloudConfigurationHelper.MoveConnectionStringsToConfig(SettingsKeys.QLogDataSource);
        }

    }
}
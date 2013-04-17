using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AzureTraining.Web.App_Start;
using AzureTraining.Web.Binders;
using AzureTraining.Web.Models;
using Microsoft.WindowsAzure;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureTraining.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            MoveConnectionStringsToConfig("DefaultConnection");
            MoveConnectionStringsToConfig("DataConnectionString");
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BootstrapBundleConfig.RegisterBundles(BundleTable.Bundles);
            
            Bootstrapper.Initialise();

            ModelBinders.Binders.Add(typeof(DocumentUploadViewModel), new DocumentUploadViewModelModelBinder());
            
            AuthConfig.RegisterAuth();

            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(ConfigurationManager.ConnectionStrings[configName].ConnectionString);
            });
        }

        private void MoveConnectionStringsToConfig(string connectionStringKey)
        {
            string connectionString = RoleEnvironment.GetConfigurationSettingValue(connectionStringKey);
            // Obtain the RuntimeConfig type. and instance
            Type runtimeConfig = Type.GetType("System.Web.Configuration.RuntimeConfig, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var runtimeConfigInstance = runtimeConfig.GetMethod("GetAppConfig", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);

            var connectionStringSection = runtimeConfig.GetProperty("ConnectionStrings", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(runtimeConfigInstance, null);
            var connectionStrings = connectionStringSection.GetType().GetProperty("ConnectionStrings", BindingFlags.Public | BindingFlags.Instance).GetValue(connectionStringSection, null);
            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(connectionStrings, false);
            // Set the SqlConnectionString property.
            ((ConnectionStringsSection)connectionStringSection).ConnectionStrings.Add(new ConnectionStringSettings(connectionStringKey, connectionString, "System.Data.SqlClient"));
        }
    }
}
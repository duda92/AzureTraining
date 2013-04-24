using System;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Reflection;
using Microsoft.WindowsAzure;

namespace AzureTraining.Core.WindowsAzure.Helpers
{
    public static class CloudConfigurationHelper
    {
        private const string ProviderName = "System.Data.SqlClient";

        public static CloudStorageAccount Account
        {
            get
            {
                return CloudStorageAccount.FromConfigurationSetting(SettingsKeys.DataConnectionString);
            }
        }

        public static string DignosticsLogsTable
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue(SettingsKeys.DignosticsLogsTable);
            }
        }

        public static string LogsTable
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue(SettingsKeys.LogsTable);
            }
        }

        public static string DocumentsTable
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue(SettingsKeys.DocumentsTable);
            }
        }
        
        public static void MoveConnectionStringsToConfig(string connectionStringKey)
        {
            string connectionString = RoleEnvironment.GetConfigurationSettingValue(connectionStringKey);
            Type runtimeConfig = Type.GetType("System.Web.Configuration.RuntimeConfig, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var runtimeConfigInstance = runtimeConfig.GetMethod("GetAppConfig", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);

            var connectionStringSection = runtimeConfig.GetProperty("ConnectionStrings", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(runtimeConfigInstance, null);
            var connectionStrings = connectionStringSection.GetType().GetProperty("ConnectionStrings", BindingFlags.Public | BindingFlags.Instance).GetValue(connectionStringSection, null);
            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(connectionStrings, false);

            ((ConnectionStringsSection)connectionStringSection).ConnectionStrings.Add(new ConnectionStringSettings(connectionStringKey, connectionString, ProviderName));
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using AzureTraceListeners.Listeners;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureTraining.Core.WindowsAzure.Helpers
{
    public static class LoggingHelper
    {
        public static void ConfigureStandartLogging()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;

            string connectionString = RoleEnvironment.GetConfigurationSettingValue(CloudConfigurationHelper.SettingsKeys.DiagnosticsConnectionString);
            Trace.Listeners.Add(new AzureTableTraceListener(RoleEnvironment.CurrentRoleInstance.Id, connectionString, "TraceLogs"));
            
            //Windows Event Logs
            dmc.WindowsEventLog.DataSources.Add("System!*");
            dmc.WindowsEventLog.DataSources.Add("Application!*");
            dmc.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromSeconds(1.0);
            dmc.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Warning;

            //Azure Trace Logs
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Warning;

            //Crash Dumps
            CrashDumps.EnableCollection(true);

            //IIS Logs
            dmc.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);

            DiagnosticMonitor.Start(CloudConfigurationHelper.SettingsKeys.DiagnosticsConnectionString, dmc);
        }
    }
}

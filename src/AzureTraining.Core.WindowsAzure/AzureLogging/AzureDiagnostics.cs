using System;
using System.Diagnostics;
using AzureTraceListeners.Listeners;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using AzureTraining.Core.WindowsAzure.Helpers;

namespace AzureTraining.Core.WindowsAzure.AzureLogging
{
    public static class AzureDiagnostics
    {
        private static readonly string DiagnosticsLogsTable;

        static AzureDiagnostics()
        {
            DiagnosticsLogsTable = CloudConfigurationHelper.DignosticsLogsTable;
        }

        /// <summary>
        /// Use it in Role OnStart() method if statndart logging required
        /// </summary>
        public static void Configure()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;

            string connectionString = RoleEnvironment.GetConfigurationSettingValue(SettingsKeys.DiagnosticsConnectionString);
            Trace.Listeners.Add(new AzureTableTraceListener(RoleEnvironment.CurrentRoleInstance.Id, connectionString, DiagnosticsLogsTable));
            
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

            DiagnosticMonitor.Start(SettingsKeys.DiagnosticsConnectionString, dmc);
        }
    }
}

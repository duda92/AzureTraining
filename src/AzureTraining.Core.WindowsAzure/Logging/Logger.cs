using System;
using System.Linq;
using AzureTraceListeners.Listeners;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;

namespace AzureTraining.Core.WindowsAzure.Logging
{
    public class Logger
    {
        public const string ErrorTable = "ErrorsLogs";
        public const string TraceTable = "TraceLogs";
        public const string InfoTable  = "InfoLogs";
        private const string InfoMessageTemplateKey = "InfoMessageTemplate";
        private const string ErrorMessageTemplateKey = "ErrorMessageTemplateKey";
        private const string TraceMessageTemplateKey = "TraceMessageTemplateKey";

        private const string RoleIdKey = "RoleId";
        private const string CustomLogsConnectionStringKey = "CustomLogsConnectionString";

        private string _roleId;
        private string _connectionString;

        private string infoMessageTemplate;
        private string errorMessageTemplate;
        private string traceMessageTemplate;

        private static Logger instance;

        private Logger() 
        {
            LoadConfiguration();
            SetUpLogging();
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        public void Info(string message, params object [] parameters)
        {
            Trace.TraceInformation(infoMessageTemplate, message, parameters);
        }

        public void Trace(string message, params object[] parameters)
        {
            Trace.TraceInformation(string.Format(infoMessageTemplate, parameters));
        }

        public void Error(string message, params object[] parameters)
        {
            Trace.TraceError(infoMessageTemplate, parameters);
        }

        private void LoadConfiguration()
        {
            _roleId = RoleEnvironment.GetConfigurationSettingValue(RoleIdKey);
            _connectionString = RoleEnvironment.GetConfigurationSettingValue(CustomLogsConnectionStringKey);

            infoMessageTemplate = RoleEnvironment.GetConfigurationSettingValue(InfoMessageTemplateKey);
            errorMessageTemplate = RoleEnvironment.GetConfigurationSettingValue(ErrorMessageTemplateKey);
            traceMessageTemplate = RoleEnvironment.GetConfigurationSettingValue(TraceMessageTemplateKey);
        }

        private void SetUpLogging()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;

            Trace.Listeners.Add(new AzureTableTraceListener(_roleId, _connectionString, ErrorTable));
            Trace.Listeners.Add(new AzureTableTraceListener(_roleId, _connectionString, TraceTable));
            Trace.Listeners.Add(new AzureTableTraceListener(_roleId, _connectionString, InfoTable));

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

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);
        }
    }
}

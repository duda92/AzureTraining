using System;
using System.Linq;

namespace AzureTraining.Core
{
    public static class SettingsKeys
    {
        public const string DefaultConnection = "DefaultConnection";
        public const string DataConnectionString = "DataConnectionString";
        public const string QLogDataSource = "QLogDataSource";
        public const string DiagnosticsConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        public const string LogsTable = "QLog";
        public const string DignosticsLogsTable = "Diagnostics";

        public static string DocumentsTable = "DocumentsTable";
    };
}

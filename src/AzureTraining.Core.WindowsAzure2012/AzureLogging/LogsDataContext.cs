using System;
using System.Linq;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.WindowsAzure.StorageClient;
using QLog.Models;

namespace AzureTraining.Core.WindowsAzure.AzureLogging
{
    public class LogsDataContext : TableServiceContext, IDisposable
    {
        public readonly string LogsTable;

        public LogsDataContext()
            : this(CloudConfigurationHelper.Account)
        {
            LogsTable = CloudConfigurationHelper.LogsTable;
        }

        public LogsDataContext(Microsoft.WindowsAzure.CloudStorageAccount account)
            : base(account.TableEndpoint.ToString(), account.Credentials)
        {
            
        }

        public IQueryable<QLogEntry> LogEntries
        {
            get
            {
                return this.CreateQuery<QLogEntry>(LogsTable);
            }
        }

        public void Dispose()
        {
            //todo: clarify how to dispose context
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.WindowsAzure.StorageClient;
using QLog.Models;

namespace AzureTraining.Core.WindowsAzure
{
    public class Logger : ILogger
    {
        public void DocumentProcessingFinished(string owner, string documentName)
        {
            QLog.Logger.LogTrace(string.Format("Document processed |{0}|{1}", owner, documentName));  
        }

        public void DocumentChangedPolicy(string owner, string documentName, bool isShared)
        {
            if (isShared)
            {
                QLog.Logger.LogTrace(string.Format("Document is Shared |{0}|{1}", owner, documentName));
            }
            else
            {
                QLog.Logger.LogTrace(string.Format("Document is Protected |{0}|{1}", owner, documentName));
            }
        }

        public void DocumentUploaded(string documentName, string login)
        {
            QLog.Logger.LogTrace(string.Format("Document uploaded to the cloud |{0}|{1}", login, documentName));     
        }

        public void UserLoggedIn(string login)
        {
            QLog.Logger.LogTrace(string.Format("User logined |{0}", login));     
        }

        public void UserLoggedOut(string login)
        {
            QLog.Logger.LogTrace(string.Format("User logged out |{0}", login));
        }

        public void UserRegistered(string login)
        {
            QLog.Logger.LogTrace(string.Format("User registered |{0}", login));
        }

        public IEnumerable<UserLog> GetLogs(string login)
        {
            using (var context = new LogsDataContext())
            {
                var logs = context.LogEntries.ToModel();
                return logs;
            }
        }
    }


    public class LogsDataContext : TableServiceContext, IDisposable
    {
        public const string LogsTable = "qlog20130418";

        public LogsDataContext()
            : this(CloudConfigurationHelper.GetAccount())
        {
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

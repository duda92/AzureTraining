using System;
using System.Collections.Generic;
using System.Linq;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.WindowsAzure.StorageClient;
using QLog.Models;
using QLog;

namespace AzureTraining.Core.WindowsAzure
{
    public class AzureLogger : IAzureLogger
    {
        public void DocumentProcessingFinished(string owner, string documentName)
        {
            var customLogInfo = new CustomLogInfo { DocumentName = documentName, User = owner, Message = "Document processed" };
            QLog.Logger.LogTrace(customLogInfo);
        }

        public void DocumentChangedPolicy(string owner, string documentName, bool isShared)
        {
            var customLogInfo = new CustomLogInfo { DocumentName = documentName, User = owner };
            customLogInfo.Message = isShared ? "Document is Shared" : "Document is Protected";
            QLog.Logger.LogTrace(customLogInfo);
        }

        public void DocumentUploaded(string documentName, string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "Document uploaded to the cloud", DocumentName = documentName, User = login };
            QLog.Logger.LogTrace(customLogInfo);     
        }

        public void UserLoggedIn(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User logined", User = login };
            QLog.Logger.LogTrace(customLogInfo);        
        }

        public void UserLoggedOut(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User logged out", User = login };
            QLog.Logger.LogTrace(customLogInfo);
        }

        public void UserRegistered(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User registered", User = login };
            QLog.Logger.LogTrace(customLogInfo);
        }

        public IEnumerable<UserLog> GetLogs(string login)
        {
            using (var context = new LogsDataContext())
            {
                var logs = context.LogEntries.Where(x => x.User == login).ToList().ToModel();
                return logs;
            }
        }

        public IEnumerable<UserLog> GetLogsForDocument(string login, string documentName)
        {
            using (var context = new LogsDataContext())
            {
                var logs = context.LogEntries.Where(x => x.User == login && x.DocumentName == documentName).ToList().ToModel();
                return logs;
            }
        }

        
    }


    public class LogsDataContext : TableServiceContext, IDisposable
    {
        public const string LogsTable = "qlog";

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

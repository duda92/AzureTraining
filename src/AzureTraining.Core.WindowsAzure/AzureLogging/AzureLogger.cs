using System.Collections.Generic;
using System.Linq;
using AzureTraining.Core.Interfaces;
using QLog;

namespace AzureTraining.Core.WindowsAzure.AzureLogging
{
    public class AzureLogger : ILogger
    {
        public void DocumentProcessingFinished(string owner, string documentName)
        {
            var customLogInfo = new CustomLogInfo { DocumentName = documentName, User = owner, Message = "Document processed" };
            Logger.LogTrace(customLogInfo);
        }

        public void DocumentChangedPolicy(string owner, string documentName, bool isShared)
        {
            var customLogInfo = new CustomLogInfo { DocumentName = documentName, User = owner };
            customLogInfo.Message = isShared ? "Document is Shared" : "Document is Protected";
            Logger.LogTrace(customLogInfo);
        }

        public void DocumentUploaded(string documentName, string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "Document uploaded to the cloud", DocumentName = documentName, User = login };
            Logger.LogTrace(customLogInfo);     
        }

        public void UserLoggedIn(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User logined", User = login };
            Logger.LogTrace(customLogInfo);        
        }

        public void UserLoggedOut(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User logged out", User = login };
            Logger.LogTrace(customLogInfo);
        }

        public void UserRegistered(string login)
        {
            var customLogInfo = new CustomLogInfo { Message = "User registered", User = login };
            Logger.LogTrace(customLogInfo);
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
}

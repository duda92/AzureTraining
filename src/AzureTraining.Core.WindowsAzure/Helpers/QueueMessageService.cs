using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureTraining.Core.WindowsAzure.Helpers
{
    public class QueueMessageService
    {
        private readonly CloudStorageAccount _storageAccount;

        public QueueMessageService(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }

        public void SendToDocumentsQueue(string documentId, string owner, string fileName)
        {
            SendToQueue(Defines.DocumentsQueue, string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", owner, documentId, fileName)); 
        }

        public void SendToDocumentsCleanupQueue(string documentId, string owner, string fileName)
        {
            SendToQueue(Defines.DocumentsCleanupQueue,string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", documentId, owner, fileName));
        }

        private void SendToQueue(string queueName, string msg)
        {
            var queues = _storageAccount.CreateCloudQueueClient();
            var q = queues.GetQueueReference(queueName);
            q.CreateIfNotExist();
            q.AddMessage(new CloudQueueMessage(msg));
        }

        public static string GetOwnerParam(CloudQueueMessage msg)
        {
            var parts = msg.AsString.Split('|');
            if (parts.Length != 3)
            {
                Trace.TraceError("Unexpected input to the photo cleanup queue: {0}", msg.AsString);
            }
            var owner = parts[0];
            return owner;
        }

        public static string GetDocumentIdParam(CloudQueueMessage msg)
        {
            var parts = msg.AsString.Split('|');
            if (parts.Length != 3)
            {
                Trace.TraceError("Unexpected input to the photo cleanup queue: {0}", msg.AsString);
            }
            var documentId = parts[1];
            return documentId;
        }

        public static string GetFileNameParam(CloudQueueMessage msg)
        {
            var parts = msg.AsString.Split('|');
            if (parts.Length != 3)
            {
                Trace.TraceError("Unexpected input to the photo cleanup queue: {0}", msg.AsString);
            }
            var fileName = parts[2];
            return fileName;
        }
            
        
    }
}

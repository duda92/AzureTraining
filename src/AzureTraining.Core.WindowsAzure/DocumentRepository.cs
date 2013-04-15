using System;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using System.Collections.Generic;

namespace AzureTraining.Core.WindowsAzure
{
    public class DocumentRepository : IDocumentRepository
    {
        private const string ContentType = "text/plain";

        private readonly CloudStorageAccount storageAccount;

        public DocumentRepository(CloudStorageAccount account)
        {
            this.storageAccount = account;
        }

        public DocumentRepository()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        {
        }

        public IEnumerable<Document> GetDocumentsByOwner(string owner)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.Owner == owner && true).AsTableServiceQuery().AsEnumerable().ToModel();
            }
        }

        public Document GetDocumentById(string owner, int documentId)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.Owner == owner && p.DocumentId == documentId && true).AsTableServiceQuery().AsEnumerable().ToModel().SingleOrDefault();
            }
        }

        public void Add(Document document, string text, string name)
        {
            //get just the file name and ignore the path
            var file = name.Substring(name.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);

            using (var context = new DocumentsDataContext())
            {
                try
                {
                    // add the photo to table storage
                    context.AddObject(DocumentsDataContext.DocumentsTable, new DocumentRow(document));
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("EntityAlreadyExists"))
                    {
                        //
                    }
                    else
                    {
                        throw;
                    }
                }

                // add the binary to blob storage
                var storage = this.storageAccount.CreateCloudBlobClient();
                var container = storage.GetContainerReference(document.Owner.ToLowerInvariant());
                container.CreateIfNotExist();
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                var blob = container.GetBlobReference(file);

                blob.Properties.ContentType = ContentType;
                blob.UploadText(text);

                // post a message to the queue so it can process tags and the sizing operations
                this.SendToQueue(
                    Defines.DocumentsQueue,
                    string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.Owner, document.DocumentId, file));
            }
        }

        public void Delete(int documentId)
        {
            using (var context = new DocumentsDataContext())
            {
                var document = context.Documents.Where(p => p.DocumentId == documentId && true).AsTableServiceQuery().ToModel().SingleOrDefault();

                if (document != null)
                {
                    this.Delete(document);
                }
            }
        }

        private void Delete(Document document)
        {
            var context = new DocumentsDataContext();
            var documentRow = new DocumentRow(document);

            context.AttachTo(DocumentsDataContext.DocumentsTable, documentRow, "*");
            context.DeleteObject(documentRow);
            context.SaveChanges();

            // tell the worker role to clean up blobs
            this.SendToQueue(
                Defines.DocumentsCleanupQueue,
                string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.DocumentId, document.Owner, document.Url));
        }

        public void Share(int documentId)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void StopShare(int documentId)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Update(Document document, string binary, string name)
        {
            using (var context = new DocumentsDataContext())
            {
                var documentRow = new DocumentRow(document);

                // attach and update the photo row
                context.AttachTo(DocumentsDataContext.DocumentsTable, documentRow, "*");
                context.UpdateObject(documentRow);
                context.SaveChanges();
            }
        }

        public void BootstrapUser(string userName)
        {
            // provision a container for the user's blobs
            var client = this.storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(userName.ToLowerInvariant());

            bool success = container.CreateIfNotExist();

            // set to public access
            container.SetPermissions(
                new BlobContainerPermissions()
                {
                    PublicAccess = BlobContainerPublicAccessType.Container
                });

            if (!success)
            {
                Trace.TraceError("Failed to create container for {0}", userName);
            }
        }

        private void SendToQueue(string queueName, string msg)
        {
            var queues = this.storageAccount.CreateCloudQueueClient();

            // TODO: add error handling and retry logic
            var q = queues.GetQueueReference(queueName);
            q.CreateIfNotExist();
            q.AddMessage(new CloudQueueMessage(msg));
        }
    }
}

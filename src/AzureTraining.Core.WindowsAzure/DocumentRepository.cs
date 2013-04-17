using System;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using System.Collections.Generic;
using AzureTraining.Core.WindowsAzure.Helpers;

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
            : this(AccountHelper.GetAccount())
        {
        }

        public IEnumerable<Document> GetAccessDocumentsForUser(string user)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.IsShared == true || p.Owner == user).AsTableServiceQuery().ToList().ToModel();
            }
        }

        public IEnumerable<Document> GetDocumentsByOwner(string owner)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.Owner == owner && true).AsTableServiceQuery().AsEnumerable().ToModel();
            }
        }

        public Document GetDocumentById(string owner, string documentId)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.Owner == owner && p.DocumentId == documentId && true).AsTableServiceQuery().AsEnumerable().ToModel().SingleOrDefault();
            }
        }

        public void Add(Document document, string text)
        {
            using (var context = new DocumentsDataContext())
            {
                SaveEntryToTable(document, context);
                var fileName = SaveBlob(document, text);
                SendToQueue(Defines.DocumentsQueue, string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.Owner, document.DocumentId, fileName));
            }
        }
  
        public void Delete(string documentId)
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

            this.SendToQueue(
                Defines.DocumentsCleanupQueue,
                string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.DocumentId, document.Owner, document.Url));
        }

        public void Share(string documentId)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void StopShare(string documentId)
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

        private void SaveEntryToTable(Document document, DocumentsDataContext context)
        {
            for (int copyNumber = 0; copyNumber < int.MaxValue; copyNumber++)
            {
                try
                {
                    SetUniqueNameAndId(document, copyNumber);
                    var docRow = new DocumentRow(document);
                    context.AddObject(DocumentsDataContext.DocumentsTable, docRow);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("EntityAlreadyExists"))
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
                break;
            }
        }

        private void SetUniqueNameAndId(Document document,  int copyNumber)
        {
            var copySuffix = copyNumber == 0 ? string.Empty : string.Format("_{0}", copyNumber);
            var identityString = document.Owner + document.Name + copySuffix;

            document.DocumentId = KeyGenerationHelper.GetSlug(identityString);
            document.Name = document.Name + copySuffix;
        }

        private string SaveBlob(Document document, string text)
        {
            var storage = this.storageAccount.CreateCloudBlobClient();
            var container = storage.GetContainerReference(document.Owner.ToLowerInvariant());
            container.CreateIfNotExist();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            var fileName = document.Name.Substring(document.Name.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);
            var blob = container.GetBlobReference(fileName);
            blob.Properties.ContentType = ContentType;
            blob.UploadText(text);
            return fileName;
        }
    }
}

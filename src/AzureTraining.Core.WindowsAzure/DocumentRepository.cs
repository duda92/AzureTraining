using System;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Collections.Generic;
using AzureTraining.Core.WindowsAzure.Helpers;

namespace AzureTraining.Core.WindowsAzure
{
    public class DocumentRepository : IDocumentRepository
    {
        private const string ContentType = "text/plain";
        private const string FileRepeatSuffixTemplate = "_{0}";

        private readonly CloudStorageAccount storageAccount;

        public DocumentRepository()
        {
            this.storageAccount = CloudConfigurationHelper.Account;
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
                return context.Documents.Where(p => p.Owner == owner && p.DocumentId == documentId || p.DocumentId == documentId && p.IsShared == true).AsTableServiceQuery().AsEnumerable().ToModel().SingleOrDefault();
            }
        }

        public void Add(Document document, string text)
        {
            SaveEntryToTable(document);
            var fileName = SaveBlob(document, text);
            SendToQueue(Defines.DocumentsQueue, string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.Owner, document.DocumentId, fileName));
            
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

            context.AttachTo(context.DocumentsTable, documentRow, "*");
            context.DeleteObject(documentRow);
            context.SaveChanges();

            this.SendToQueue(
                Defines.DocumentsCleanupQueue,
                string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", document.DocumentId, document.Owner, document.Url));
        }

        public void Update(Document document)
        {
            using (var context = new DocumentsDataContext())
            {
                var documentRow = new DocumentRow(document);
                context.AttachTo(context.DocumentsTable, documentRow, "*");
                context.UpdateObject(documentRow);
                context.SaveChanges();
            }
        }

        public string GetPageContent(string owner, string documentId, string fileName, int page)
        {
            using (var context = new DocumentsDataContext())
            {
                var docs = context.Documents.Where(p => p.Owner == owner).ToModel();
                var doc = docs.FirstOrDefault();
                var documentContent = GetDocumentText(doc);
                var paginationService = new PaginationService();
                var content = paginationService.GetDocumentPage(documentContent, page);
                return content;
            }
        }

        private void SendToQueue(string queueName, string msg)
        {
            var queues = this.storageAccount.CreateCloudQueueClient();
            var q = queues.GetQueueReference(queueName);
            q.CreateIfNotExist();
            q.AddMessage(new CloudQueueMessage(msg));
        }

        private void SaveEntryToTable(Document document)
        {
            for (int copyNumber = 0; copyNumber < int.MaxValue; copyNumber++)
            {
                try
                {
                    using (var context = new DocumentsDataContext())
                    {
                        SetUniqueNameAndId(document, copyNumber);
                        var docRow = new DocumentRow(document);
                        context.AddObject(context.DocumentsTable, docRow);
                        context.SaveChanges();
                    }
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
            string extension = System.IO.Path.GetExtension(document.Name);
            RemovePreviousSuffix(copyNumber, document, extension);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(document.Name);
            
            var copySuffix = copyNumber == 0 ? string.Empty : string.Format(FileRepeatSuffixTemplate, copyNumber);
            var identityString = document.Owner + fileName + copySuffix + extension;

            document.DocumentId = KeyGenerationHelper.GetSlug(identityString);
            document.Name = fileName + copySuffix + extension;
        }
  
        private void RemovePreviousSuffix(int copyNumber, Document document, string extension)
        {
            if (copyNumber > 1)
            {
                document.Name = document.Name.Replace(string.Format(FileRepeatSuffixTemplate + extension, copyNumber - 1), extension);
            }
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

        private string GetDocumentText(Document document)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(document.Owner);
            var blob = container.GetBlobReference(document.Name);
            var text = blob.DownloadText();
            return text;
        }
    }
}

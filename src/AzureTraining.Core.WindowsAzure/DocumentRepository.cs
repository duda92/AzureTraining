using System;
using System.Linq;
using AzureTraining.Core.Interfaces;
using AzureTraining.Core.Services;
using Microsoft.WindowsAzure;
using System.Collections.Generic;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureTraining.Core.WindowsAzure
{
    public class DocumentRepository : IDocumentRepository
    {
        private const string ContentType = "text/plain";
        private readonly CloudStorageAccount _storageAccount;
        private readonly QueueMessageService _queueMessageService;
        
        public DocumentRepository()
        {
            _storageAccount = CloudConfigurationHelper.Account;
            _queueMessageService = new QueueMessageService(_storageAccount);
        }

        public IEnumerable<Document> GetAccessDocumentsForUser(string user)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.IsShared || p.Owner == user).AsTableServiceQuery().ToList().ToModel();
            }
        }

        public IEnumerable<Document> GetDocumentsByOwner(string owner)
        {
            using (var context = new DocumentsDataContext())
            {
                return context.Documents.Where(p => p.Owner == owner).AsTableServiceQuery().AsEnumerable().ToModel();
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
            _queueMessageService.SendToDocumentsQueue(document.DocumentId, document.Owner, fileName); 
        }
  
        public void Delete(string documentId)
        {
            using (var context = new DocumentsDataContext())
            {
                var document = context.Documents.Where(p => p.DocumentId == documentId && true).AsTableServiceQuery().ToModel().SingleOrDefault();
                if (document != null)
                {
                    Delete(document);
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

            _queueMessageService.SendToDocumentsCleanupQueue(document.DocumentId, document.Owner, document.OriginFileUrl);
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

        public string GetPageContent(string owner, string documentId, int page)
        {
            using (var context = new DocumentsDataContext())
            {
                var docs = context.Documents.Where(p => p.Owner == owner && p.DocumentId == documentId).ToModel();
                var doc = docs.FirstOrDefault();
                if (doc == null)
                    throw new ArgumentNullException("document", "document not found");
                var documentContent = GetProcessedDocumentText(doc);
                var paginationService = new PaginationService();
                var content = paginationService.GetDocumentPage(documentContent, page);
                return content;
            }
        }

        private static void SaveEntryToTable(Document document)
        {
            for (var copyNumber = 0; copyNumber < int.MaxValue; copyNumber++)
            {
                try
                {
                    using (var context = new DocumentsDataContext())
                    {
                        NamingHelper.AttachCopyNumberToFileName(document, copyNumber);
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
                    throw;
                }
                break;
            }
        }

        private string SaveBlob(Document document, string text)
        {
            var storage = _storageAccount.CreateCloudBlobClient();
            var container = storage.GetContainerReference(document.Owner.ToLowerInvariant());
            container.CreateIfNotExist();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            var fileName = NamingHelper.RemoveFullPath(document.Name);
            var blob = container.GetBlobReference(fileName);
            blob.Properties.ContentType = ContentType;
            blob.UploadText(text);
            return fileName;
        }

        private string GetProcessedDocumentText(Document document)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(document.Owner);
            var blob = container.GetBlobReference(NamingHelper.GetProcessedFileName(document.Name));
            var text = blob.DownloadText();
            return text;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTraining.Core
{
    public interface IDocumentRepository
    {
        IEnumerable<Document> GetDocumentsByOwner(string owner);

        IEnumerable<Document> GetAccessDocumentsForUser(string user);

        Document GetDocumentById(string owner, string documentId);
        
        void Add(Document document, string text);

        void Update(Document document);

        void Share(string documentId);

        void StopShare(string documentId);

        void Delete(string documentId);

        void BootstrapUser(string userName);

        
    }
}

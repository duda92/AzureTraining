using System.Collections.Generic;

namespace AzureTraining.Core.Interfaces
{
    public interface IDocumentRepository
    {
        IEnumerable<Document> GetDocumentsByOwner(string owner);

        IEnumerable<Document> GetAccessDocumentsForUser(string user);

        Document GetDocumentById(string owner, string documentId);
        
        string GetPageContent(string owner, string documentId, int page);

        void Add(Document document, string text);

        void Update(Document document);

        void Delete(string documentId);
    }
}

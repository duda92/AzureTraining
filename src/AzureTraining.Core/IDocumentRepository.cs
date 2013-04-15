using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTraining.Core
{
    public interface IDocumentRepository
    {
        IEnumerable<Document> GetDocumentsByOwner(string owner);

        Document GetDocumentById(string owner, int documentId);
        
        void Add(Document document, string text, string name);

        void Update(Document document, string text, string name);

        void Share(int documentId);

        void StopShare(int documentId);

        void Delete(int documentId);

        void BootstrapUser(string userName);
    }
}

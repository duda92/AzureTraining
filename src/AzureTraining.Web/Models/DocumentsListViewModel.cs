using AzureTraining.Core;
using System.Collections.Generic;

namespace AzureTraining.Web.Models
{
    public class DocumentsListViewModel
    {
        public IEnumerable<Document> Documents { get; set; }

        public DocumentsListViewModel(IEnumerable<Document> documents)
        {
            Documents = documents;
        }
    }
}
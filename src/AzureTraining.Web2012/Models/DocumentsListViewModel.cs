using AzureTraining.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTraining.Web.Models
{
    public class DocumentsListViewModel
    {
        public IEnumerable<Document> Documents { get; set; }

        public DocumentsListViewModel(IEnumerable<Document> documents)
        {
            this.Documents = documents;
        }
    }
}
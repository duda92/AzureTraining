using System;
using System.Linq;

namespace AzureTraining.Web.Models
{
    public class DocumentPageViewViewModel
    {
        public string Text { get; set; }

        public int Page { get; set; }

        public string DocumentName { get; set; }

        public string DocumentId { get; set; }
    }
}
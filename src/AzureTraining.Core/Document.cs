using System;
using System.Linq;

namespace AzureTraining.Core
{
    public class Document
    {
        public Document()
        {
            DocumentId = string.Empty;
            Preview = string.Empty;
        }

        public string DocumentId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Owner { get; set; }

        public bool IsShared { get; set; }

        public string Preview { get; set; }
    }
}

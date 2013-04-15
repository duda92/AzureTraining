using System;
using System.Linq;

namespace AzureTraining.Web.Models
{
    public class DocumentUploadViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShared { get; set; }

        public string Content { get; set; }
    }
}
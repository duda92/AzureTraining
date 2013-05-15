using AzureTraining.Core;

namespace AzureTraining.Web.Models
{
    public class DocumentViewViewModel
    {
        public Document Document { get; set; }

        public string PageContent { get; set; }

        public int PageNumber { get; set; }
    }
}
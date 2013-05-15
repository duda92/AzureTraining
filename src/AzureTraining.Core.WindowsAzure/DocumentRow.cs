using Microsoft.WindowsAzure.StorageClient;

namespace AzureTraining.Core.WindowsAzure
{
    using System.Globalization;
    using Core;

    public class DocumentRow : TableServiceEntity
    {
        public string DocumentId { get; set; }

        public string Name { get; set; }

        public string OriginalFileUrl { get; set; }

        public string ProcessedFileUrl { get; set; }

        public string Owner { get; set; }

        public bool IsShared { get; set; }

        public string Preview { get; set; }

        public int PagesCount { get; set; }

        public DocumentRow() 
        {
        }

        public DocumentRow(Document document)
            : base(string.Format(CultureInfo.InvariantCulture, "{0}", document.Owner), document.DocumentId.ToString(CultureInfo.InvariantCulture))
        {
            DocumentId = document.DocumentId;
            Name = document.Name;
            Owner = document.Owner;
            OriginalFileUrl = document.OriginFileUrl;
            IsShared = document.IsShared;
            PartitionKey = document.DocumentId;
            Preview = document.Preview;
            PagesCount = document.PagesCount;
            ProcessedFileUrl = document.ProcessedFileUrl;
        }
    }
}

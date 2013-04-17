namespace AzureTraining.Core.WindowsAzure
{
    using System;
    using System.Globalization;
    using Microsoft.WindowsAzure.StorageClient;
    using AzureTraining.Core;

    public class DocumentRow : TableServiceEntity
    {
        public string DocumentId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Owner { get; set; }

        public bool IsShared { get; set; }

        public DocumentRow() : base()
        {
        }

        public DocumentRow(Document document)
            : base(string.Format(CultureInfo.InvariantCulture, "{0}", document.Owner), document.DocumentId.ToString())
        {
            this.DocumentId = document.DocumentId;
            this.Name = document.Name;
            this.Owner = document.Owner;
            this.Url = document.Url;
            this.IsShared = document.IsShared;
            this.PartitionKey = document.DocumentId;
        }

        private DocumentRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTraining.Core.WindowsAzure
{
    public static class DataExtensions
    {
        public static IEnumerable<Document> ToModel(this IEnumerable<DocumentRow> rows)
        {
            if (rows.ToList() == null)
                rows = new List<DocumentRow> { };
            else
            {
                foreach (var row in rows)
                {
                    yield return row.ToModel();
                }
            }
        }

        public static Document ToModel(this DocumentRow row)
        {
            return new Document()
            {
                DocumentId = row.DocumentId,
                Owner = row.Owner,
                Name = row.Name,
                Url = row.Url,
                IsShared = row.IsShared,
                Preview = row.Preview
            };
        }
    }
}

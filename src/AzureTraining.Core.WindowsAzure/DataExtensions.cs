using QLog.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureTraining.Core.WindowsAzure
{
    public static class DataExtensions
    {
        public static IEnumerable<Document> ToModel(this IEnumerable<DocumentRow> rows)
        {
            return rows.Select(row => row.ToModel());
        }

        public static Document ToModel(this DocumentRow row)
        {
            return new Document
            {
                DocumentId = row.DocumentId,
                Owner = row.Owner,
                Name = row.Name,
                OriginFileUrl = row.OriginalFileUrl,
                ProcessedFileUrl = row.ProcessedFileUrl,
                IsShared = row.IsShared,
                Preview = row.Preview,
                PagesCount = row.PagesCount
            };
        }

        public static IEnumerable<UserLog> ToModel(this IEnumerable<QLogEntry> rows)
        {
            return rows.Select(row => row.ToModel());
        }

        public static UserLog ToModel(this QLogEntry row)
        {
            return new UserLog()
            {
                User = row.User,
                Date = row.CreatedOn,
                DocumentId = row.DocumentId, 
                DocumentName = row.DocumentName,
                Message = row.Message
            };
        }
    }
}

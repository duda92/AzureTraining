﻿using QLog.Models;
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
                Preview = row.Preview,
                PagesCount = row.PagesCount
            };
        }

        public static IEnumerable<UserLog> ToModel(this IEnumerable<QLogEntry> rows)
        {
            if (rows.ToList() == null)
                rows = new List<QLogEntry> { };
            else
            {
                foreach (var row in rows)
                {
                    yield return row.ToModel();
                }
            }
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

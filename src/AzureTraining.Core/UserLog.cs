using System;

namespace AzureTraining.Core
{
    public class UserLog
    {
        public string User { get; set; }

        public string Message { get; set; }

        public string DocumentName { get; set; }

        public string DocumentId { get; set; }

        public DateTime Date { get; set; }

    }
}

using System;
using System.Linq;

namespace AzureTraining.Core.WindowsAzure
{
    public static class Defines
    {
        public const string DocumentsQueue = "documents";

        public const string DocumentsCleanupQueue = "cleanupdocuments";

        public const int DocumentPreviewLenght = 200;

        public const int DocumentPageLenght = 3000;
    }
}

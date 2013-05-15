namespace AzureTraining.Core.WindowsAzure
{
    public static class Defines
    {
        public const string DocumentsQueue = "documents";

        public const string DocumentsCleanupQueue = "cleanupdocuments";

        public const int DocumentPreviewLenght = 200;

        public const string ContentType = "text/plain";
        
        public static class UiDefines
        {
            public const string UploadFileInputId = "file";
            public const string UploadFilePreviewId = "Content";
            
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AzureTraining.Core.Interfaces;
using AzureTraining.Core.Services;
using AzureTraining.Core.WindowsAzure;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using AzureTraining.Core;
using AzureTraining.Core.WindowsAzure.AzureLogging;

namespace AzureTraining.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly ILogger _logger = new AzureLogger();
        private readonly IPaginationService _paginator = new PaginationService();
        private readonly ITransliterationService _transliterator = new TransliterationService();
        private const string ContentType = "text/plain";
        
        private QueueClient _client;

        public WorkerRole()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

                RoleEnvironment.Changed += (sender, arg) =>
                {
                    if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
                        .Any((change) => (change.ConfigurationSettingName == configName)))
                    {
                        if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                        {
                            RoleEnvironment.RequestRecycle();
                        }
                    }
                };
            });

            _storageAccount = CloudConfigurationHelper.Account;
            //AzureDiagnostics.Configure();
        }

        
        public override void Run()
        {
            var queueClient = _storageAccount.CreateCloudQueueClient();
            int sleepTime = GetSleepTimeFromConfig();

            while (true)
            {
                Thread.Sleep(sleepTime);

                foreach (var q in queueClient.ListQueues())
                {
                    var msg = q.GetMessage();
                    var success = false;

                    if (msg != null)
                    {
                        switch (q.Name)
                        {
                            case Defines.DocumentsQueue:
                                Trace.TraceInformation("Starting process document");
                                success = ProcessDocument(msg);
                                break;
                            default:
                                Trace.TraceError("Unknown Queue found: {0}", q.Name);
                                break;
                        }
                        if (success || msg.DequeueCount > 4)
                        {
                            try
                            {
                                q.DeleteMessage(msg);
                            }
                            catch
                            {
                                Trace.TraceError("Error while deleting msg from the bus queue", q.Name);
                            }
                        }
                    }
                }
            }
        }
  
        private bool ProcessDocument(CloudQueueMessage msg)
        {
            var parts = msg.AsString.Split('|');

            if (parts.Length != 3)
            {
                Trace.TraceError("Unexpected input to the photo cleanup queue: {0}", msg.AsString);
                return false;
            }
            var owner = parts[0];
            var documentId = parts[1];
            var fileName = parts[2];

            var repository = new DocumentRepository();
            var document = repository.GetDocumentById(owner, documentId);

            if (document != null)
            {
                try
                {
                    var container = GetOwnerContainer(document);
                    var originalFileBlob = GetOriginalFileBlob(container, fileName);
                    var processedFileBlob = CreateProcessedFileBlob(container, fileName);

                    TransleteDocument(processedFileBlob);
                    SetDocumentPreview(document, processedFileBlob);
                    PaginateDocument(document, processedFileBlob);  
                    SetUri(originalFileBlob, processedFileBlob, document);
                    repository.Update(document);

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    return false; 
                }
                _logger.DocumentProcessingFinished(document.Owner, document.Name);
                return true;
            }
            Trace.TraceError("Processing document error, cannot find {0}", documentId);
            return false;
        }

        private CloudBlobContainer GetOwnerContainer(Document document)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(document.Owner);
            return container;
        }

        private CloudBlob GetOriginalFileBlob(CloudBlobContainer container, string fileName)
        {
            var originalFileBlob = container.GetBlobReference(fileName);
            return originalFileBlob;
        }

        private static CloudBlob CreateProcessedFileBlob(CloudBlobContainer container, string originalFileName)
        {
            var originFileBlob = container.GetBlobReference(originalFileName);
            var content = originFileBlob.DownloadText();
            
            var fileName = originalFileName + "_processed";
            var processedFileBlob = container.GetBlobReference(fileName);
            processedFileBlob.Properties.ContentType = ContentType;

            processedFileBlob.UploadText(content);
            return processedFileBlob;
        }

        #region DocumentProcessing

        private static void SetUri(CloudBlob originalFileBlob, CloudBlob processedFileBlob, Document document)
        {
            var originalFileUrl  = originalFileBlob.Uri.ToString();
            var processedFileUrl = processedFileBlob.Uri.ToString();

            document.OriginFileUrl = originalFileUrl;
            document.ProcessedFileUrl = processedFileUrl;
        }

        private void PaginateDocument(Document doc, CloudBlob blob)
        {
            var content = blob.DownloadText();
            var processed = _paginator.Paginate(content);
            var pagesCount = _paginator.GetPagesCount(processed);
            doc.PagesCount = pagesCount;
            blob.UploadText(processed);
        }
  
        private void TransleteDocument(CloudBlob blob)
        {
            var content = blob.DownloadText();
            var processed = _transliterator.Front(content);
            blob.UploadText(processed);
        }

        private static void SetDocumentPreview(Document doc, CloudBlob blob)
        {
            var content = blob.DownloadText();
            var preview = content.Substring(0, Math.Min(Defines.DocumentPreviewLenght, content.Length));
            doc.Preview = preview;
        }

        #endregion

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(Defines.DocumentsQueue))
            {
                namespaceManager.CreateQueue(Defines.DocumentsQueue);
            }

            _client = QueueClient.CreateFromConnectionString(connectionString, Defines.DocumentsQueue);
            return base.OnStart();
        }

        public override void OnStop()
        {
            _client.Close();
            base.OnStop();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Required to access Windows Azure Environment")]
        private static int GetSleepTimeFromConfig()
        {
            int sleepTime;

            if (!int.TryParse(RoleEnvironment.GetConfigurationSettingValue("WorkerSleepTime"), out sleepTime))
            {
                sleepTime = 0;
            }

            if (sleepTime < 1000)
            {
                sleepTime = 2000;
            }

            return sleepTime;
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
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
        private readonly CloudStorageAccount storageAccount;
        private readonly ILogger _logger = new AzureLogger();
        private readonly IPaginationService _paginator = new PaginationService();
        private readonly ITransliterationService _transliterator = new TransliterationService();
        
        private QueueClient _client;
        private bool _isStopped;
            
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

            this.storageAccount = CloudConfigurationHelper.Account;
            //AzureDiagnostics.Configure();
        }

        
        public override void Run()
        {
            var queueClient = this.storageAccount.CreateCloudQueueClient();

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
                        var id = msg.Id;
                        switch (q.Name)
                        {
                            case Defines.DocumentsQueue:
                                Trace.TraceInformation("Starting process document");
                                success = this.ProcessDocument(msg);
                                break;
                            default:
                                Trace.TraceError("Unknown Queue found: {0}", q.Name);
                                break;
                        }
                        if (success || msg.DequeueCount > 4)
                        {
                            q.DeleteMessage(msg);
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
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(document.Owner);
                    var blob = container.GetBlobReference(fileName);

                    TransleteDocument(blob);
                    
                    SetDocumentPreview(document, blob);
                    
                    PaginateDocument(document, blob);
                    
                    SetUri(blob, document);
                    
                    repository.Update(document);

                }
                catch (Exception ex)
                {
                    Trace.TraceError("SetDocumentPreview failed for {0} and {1}", owner, fileName);
                    Trace.TraceError(ex.ToString());

                    return false; 
                }
                _logger.DocumentProcessingFinished(document.Owner, document.Name);
                return true;
            }

            Trace.TraceError("Processing document error, cannot find {0}", documentId);
            return false;
        }
  
        #region DocumentProcessing

        private void SetUri(CloudBlob blob, Document document)
        {
            var blobUri = blob.Uri.ToString();
            document.Url = blobUri;
        }

        private void PaginateDocument(Document doc, CloudBlob blob)
        {
            var content = blob.DownloadText();
            string processed = _paginator.Paginate(content);
            int pagesCount = _paginator.GetPagesCount(processed);
            doc.PagesCount = pagesCount;
            blob.UploadText(processed);
        }
  
        private void TransleteDocument(CloudBlob blob)
        {
            var content = blob.DownloadText();
            string processed = _transliterator.Front(content);
            blob.UploadText(processed);
        }

        private void SetDocumentPreview(Document doc, CloudBlob blob)
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
            _isStopped = false;
            return base.OnStart();
        }

        public override void OnStop()
        {
            _isStopped = true;
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AzureTraining.Core.WindowsAzure;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using AzureTraining.Core;

namespace AzureTraining.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly IAzureLogger _logger = new AzureLogger();

        public WorkerRole()
        {
            // This code sets up a handler to update CloudStorageAccount instances when their corresponding
            // configuration settings change in the service configuration file.
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                // Provide the configSetter with the initial value
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

                RoleEnvironment.Changed += (sender, arg) =>
                {
                    if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
                        .Any((change) => (change.ConfigurationSettingName == configName)))
                    {
                        // The corresponding configuration setting has changed, propagate the value
                        if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                        {
                            // In this case, the change to the storage account credentials in the
                            // service configuration is significant enough that the role needs to be
                            // recycled in order to use the latest settings. (for example, the 
                            // endpoint has changed)
                            RoleEnvironment.RequestRecycle();
                        }
                    }
                };
            });

            this.storageAccount = CloudConfigurationHelper.GetAccount();
            LoggingHelper.ConfigureStandartLogging();
        }

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        bool IsStopped;

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

                        // remove the message if no error has occurred
                        // or delete if the message is poison (> 4 reads)
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
            var paginator = new PaginationService();
            int pagesCount;
            string processed = paginator.Paginate(content, out pagesCount);
            doc.PagesCount = pagesCount;
            blob.UploadText(processed);
        }
  
        private void TransleteDocument(CloudBlob blob)
        {
            var content = blob.DownloadText();
            string processed = Transliteration.Front(content);
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
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(Defines.DocumentsQueue))
            {
                namespaceManager.CreateQueue(Defines.DocumentsQueue);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, Defines.DocumentsQueue);
            IsStopped = false;
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
            Client.Close();
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

            // polling less than a second seems too eager
            if (sleepTime < 1000)
            {
                sleepTime = 2000;
            }

            return sleepTime;
        }

        private void SendToQueue(string queueName, string msg)
        {
            var queues = this.storageAccount.CreateCloudQueueClient();

            // TODO: add error handling and retry logic
            var q = queues.GetQueueReference(queueName);
            q.CreateIfNotExist();
            q.AddMessage(new CloudQueueMessage(msg));
        }
    }
}

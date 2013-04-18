namespace AzureTraining.Core.WindowsAzure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using AzureTraining.Core.WindowsAzure.Helpers;

    public class DocumentsDataContext : TableServiceContext, IDisposable
    {
        public const string DocumentsTable = "Documents";
        
        private static bool initialized;
        private static readonly object initializationLock = new object();

        private readonly Dictionary<string, Type> resolverTypes;

        public DocumentsDataContext()
            : this(CloudConfigurationHelper.GetAccount())
        {
        }

        public DocumentsDataContext(Microsoft.WindowsAzure.CloudStorageAccount account)
            : base(account.TableEndpoint.ToString(), account.Credentials)
        {
            if (!initialized)
            {
                lock (initializationLock)
                {
                    if (!initialized)
                    {
                        this.CreateTables();
                        initialized = true;
                    }
                }
            }

            // we are setting up a dictionary of types to resolve in order
            // to workaround a performance bug during serialization
            this.resolverTypes = new Dictionary<string, Type>();
            this.resolverTypes.Add(DocumentsTable, typeof(DocumentRow));

            this.ResolveType = (name) =>
            {
                var parts = name.Split('.');
                if (parts.Length == 2)
                {
                    var processedKey = UppercaseFirst(parts[1]);
                    return resolverTypes[processedKey];
                }

                return null;
            };
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public IQueryable<DocumentRow> Documents
        {
            get
            {
                return this.CreateQuery<DocumentRow>(DocumentsTable);
            }
        }

        public void CreateTables(Type serviceContextType, string baseAddress, StorageCredentials credentials)
        {
            TableStorageExtensionMethods.CreateTablesFromModel(serviceContextType, baseAddress, credentials);
        }

        public void CreateTables()
        {
            TableStorageExtensionMethods.CreateTablesFromModel(typeof(DocumentsDataContext), this.BaseUri.AbsoluteUri, this.StorageCredentials);
        }

        public void Dispose()
        {
            //TableServiceContext.Dispose();
        }
    }
}


using Microsoft.WindowsAzure.StorageClient;

namespace AzureTraining.Core.WindowsAzure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Helpers;

    public class DocumentsDataContext : TableServiceContext, IDisposable
    {
        public  readonly string DocumentsTable;
        private readonly Dictionary<string, Type> _resolverTypes;

        private static bool _initialized;
        private static readonly object InitializationLock = new object();

        
        public DocumentsDataContext()
            : this(CloudConfigurationHelper.Account)
        {
            
        }

        public DocumentsDataContext(CloudStorageAccount account)
            : base(account.TableEndpoint.ToString(), account.Credentials)
        {
            DocumentsTable = CloudConfigurationHelper.DocumentsTable;
            if (!_initialized)
            {
                lock (InitializationLock)
                {
                    if (!_initialized)
                    {
                        CreateTables();
                        _initialized = true;
                    }
                }
            }

            // we are setting up a dictionary of types to resolve in order
            // to workaround a performance bug during serialization
            _resolverTypes = new Dictionary<string, Type> {{DocumentsTable, typeof (DocumentRow)}};
            ResolveType = (name) =>
            {
                var parts = name.Split('.');
                if (parts.Length == 2)
                {
                    var processedKey = UppercaseFirst(parts[1]);
                    return _resolverTypes[processedKey];
                }
                return null;
            };
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public IQueryable<DocumentRow> Documents
        {
            get
            {
                return CreateQuery<DocumentRow>(DocumentsTable);
            }
        }

        public void CreateTables(Type serviceContextType, string baseAddress, StorageCredentials credentials)
        {
            TableStorageExtensionMethods.CreateTablesFromModel(serviceContextType, baseAddress, credentials);
        }

        public void CreateTables()
        {
            TableStorageExtensionMethods.CreateTablesFromModel(typeof(DocumentsDataContext), BaseUri.AbsoluteUri, StorageCredentials);
        }

        public void Dispose()
        {
            
        }
    }
}


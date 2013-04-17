using System;
using System.Linq;
using Microsoft.WindowsAzure;

namespace AzureTraining.Core.WindowsAzure.Helpers
{
    public static class AccountHelper
    {
        public static CloudStorageAccount GetAccount()
        {
            return CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
        }
    }
}

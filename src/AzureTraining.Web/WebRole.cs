using System;
using System.Linq;
using AzureTraining.Core.WindowsAzure.Helpers;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureTraining.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            LoggingHelper.ConfigureStandartLogging();
            return base.OnStart();
        }
    }
}

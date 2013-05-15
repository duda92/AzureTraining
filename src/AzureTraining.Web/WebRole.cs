using AzureTraining.Core.WindowsAzure.AzureLogging;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureTraining.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            //AzureDiagnostics.Configure();
            return base.OnStart();
        }
    }
}

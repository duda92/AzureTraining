using AzureTraining.Core.Interfaces;
using AzureTraining.Core.WindowsAzure;
using AzureTraining.Core.WindowsAzure.AzureLogging;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using Unity.Mvc4;

namespace AzureTraining.Web
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IDocumentRepository, DocumentRepository>();
            container.RegisterType<ILogger, AzureLogger>();           
            return container;
        }
    }
}
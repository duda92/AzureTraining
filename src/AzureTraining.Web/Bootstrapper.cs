using System.Web.Http;
using AzureTraining.Core;
using AzureTraining.Core.WindowsAzure;
using Microsoft.Practices.Unity;

namespace AzureTraining.Web
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IDocumentRepository, DocumentRepository>();            

            return container;
        }
    }
}
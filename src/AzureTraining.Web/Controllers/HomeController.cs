using System;
using System.Linq;
using System.Web.Mvc;
using AzureTraining.Core;
using AzureTraining.Web.Models;
using AzureTraining.Core.WindowsAzure;

namespace AzureTraining.Web.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly IDocumentRepository _repository;

        public HomeController(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public HomeController() : this(new DocumentRepository())
        {
            
        }

        public virtual ActionResult Index()
        {
            QLog.Logger.LogTrace("/Home/Index called");

            var user = User.Identity.Name;
            var documents = _repository.GetAccessDocumentsForUser(user);
            var documentsListViewModel = new DocumentsListViewModel(documents);

            return View(documentsListViewModel);
        }
    }
}

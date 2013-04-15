using System;
using System.Linq;
using System.Web.Mvc;
using AzureTraining.Core;
using AzureTraining.Web.Models;
using AzureTraining.Core.WindowsAzure;

namespace AzureTraining.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDocumentRepository _repository;

        public HomeController(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public HomeController() : this(new DocumentRepository())
        {
            
        }

        public ActionResult Upload()
        {
            DocumentUploadViewModel documentViewModel = new DocumentUploadViewModel {};

            return View(documentViewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string owner = "defaultOwner";//User.Identity.Name.ToLowerInvariant() == string.Empty ? User.Identity.Name.ToLowerInvariant() : 
            
            _repository.Add(new Document { 
                Description = model.Description,
                Owner = owner,
                IsShared = model.IsShared
            }, model.Content, model.Name);

            return RedirectToAction("Upload", "Home");
        }
    }
}

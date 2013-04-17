using AzureTraining.Core;
using AzureTraining.Core.WindowsAzure;
using AzureTraining.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AzureTraining.Web.Controllers
{
    public partial class DocumentUploadController : Controller
    {
        private readonly IDocumentRepository _repository;

        public DocumentUploadController(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public DocumentUploadController()
            : this(new DocumentRepository())
        {
            
        }
        public virtual ActionResult Upload()
        {
            DocumentUploadViewModel documentViewModel = new DocumentUploadViewModel { };
            return View(documentViewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public virtual ActionResult Upload(DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string owner = User.Identity.Name.ToLowerInvariant();

            _repository.Add(new Document
            {
                Name = model.Name,
                Owner = owner,
                IsShared = model.IsShared
            }, model.Content);

            return RedirectToAction("Upload", "Home");
        }

    }
}

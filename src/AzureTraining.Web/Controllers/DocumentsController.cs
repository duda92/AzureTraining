using AzureTraining.Core;
using AzureTraining.Core.WindowsAzure;
using AzureTraining.Web.Helpers;
using AzureTraining.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AzureTraining.Web.Controllers
{
    public partial class DocumentsController : Controller
    {
        private readonly IDocumentRepository _repository;

        public DocumentsController(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public DocumentsController()
            : this(new DocumentRepository())
        {
            
        }

        [Authorize]
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

            var document = new Document
            {
                Name = model.Name,
                IsShared = model.IsShared
            };

            DocRolesHelper.SetCurrentUserAsOwnerOfDocument(document);

            _repository.Add(document, model.Content);

            return RedirectToAction(MVC.Home.Index());
        }

        [Authorize]
        public virtual ActionResult View(string documentId)
        {
            var owner = DocRolesHelper.CurrentOwnerKey;
            var document =_repository.GetDocumentById(owner, documentId);
            return View(document);
        }
    }
}

using AzureTraining.Core;
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
        private readonly ILogger _logger;

        public DocumentsController(IDocumentRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
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

            _logger.DocumentUploaded(document.Name, document.Owner);
            return RedirectToAction(MVC.Home.Index());
        }

        public virtual ActionResult View(string documentId)
        {
            var owner = DocRolesHelper.CurrentOwnerKey;
            var document =_repository.GetDocumentById(owner, documentId);

            var viewModel = new DocumentViewViewModel
            {
                Document = document
            };

            return View(viewModel);
        }

        public virtual ActionResult ViewDocumentPage(string fileName, string documentId, int page)
        {
            var owner = DocRolesHelper.CurrentOwnerKey;

            var content = _repository.GetPageContent(owner, documentId, fileName, page);

            var viewModel = new DocumentPageViewViewModel
            {
                Text = content,
                DocumentName = fileName,
                DocumentId = documentId, 
                Page = page
            };

            return View(viewModel);
        }

        [Authorize]
        public virtual ActionResult ChangeViewPolicy(string documentId, bool isShared)
        {
            var owner = DocRolesHelper.CurrentOwnerKey;
            var document = _repository.GetDocumentById(owner, documentId);
            document.IsShared = isShared;
            _repository.Update(document);
            _logger.DocumentChangedPolicy(owner, document.Name, document.IsShared);
            return RedirectToAction(MVC.Home.Index());
        }
    }
  
    
}

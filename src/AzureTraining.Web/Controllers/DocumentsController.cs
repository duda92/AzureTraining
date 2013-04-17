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
        private readonly IDocumentRepository _repository = new DocumentRepository();

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

            var viewModel = new DocumentViewViewModel
            {
                Document = document
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
            return RedirectToAction(MVC.Home.Index());
        }
    }
}

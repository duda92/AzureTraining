using System.Web;
using AzureTraining.Core;
using System;
using System.Linq;

namespace AzureTraining.Web.Helpers
{
    public static class DocRolesHelper
    {
        public static bool IsCurrentUserOwnerOfDocument(Document doc)
        {
            var user = HttpContext.Current.User;
            return doc.Owner == user.Identity.Name.ToLowerInvariant();
        }

        public static void SetCurrentUserAsOwnerOfDocument(Document doc)
        {
            var user = HttpContext.Current.User;
            doc.Owner = user.Identity.Name.ToLowerInvariant();
        }
    }
}
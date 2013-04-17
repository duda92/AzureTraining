using System.Web;
using AzureTraining.Core;
using System;
using System.Linq;

namespace AzureTraining.Web.Helpers
{
    public static class DocRolesHelper
    {
        public static string CurrentOwnerKey
        {
            get
            {
                var user = HttpContext.Current.User;
                return user.Identity.Name.ToLowerInvariant();
            }
        }

        public static bool IsCurrentUserOwnerOfDocument(Document doc)
        {
            return doc.Owner == CurrentOwnerKey;
        }

        public static void SetCurrentUserAsOwnerOfDocument(Document doc)
        {
            doc.Owner = CurrentOwnerKey;
        }
    }
}
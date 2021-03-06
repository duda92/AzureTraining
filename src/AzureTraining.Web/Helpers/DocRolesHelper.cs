﻿using System.Web;
using AzureTraining.Core;

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

        public static string GetCurrentUserAsOwner()
        {
            return CurrentOwnerKey;
        }
    }
}
// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace AzureTraining.Web.Controllers
{
    public partial class DocumentsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected DocumentsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult View()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.View);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ChangeViewPolicy()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChangeViewPolicy);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public DocumentsController Actions { get { return MVC.Documents; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Documents";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Documents";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Upload = "Upload";
            public readonly string View = "View";
            public readonly string ChangeViewPolicy = "ChangeViewPolicy";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Upload = "Upload";
            public const string View = "View";
            public const string ChangeViewPolicy = "ChangeViewPolicy";
        }


        static readonly ActionParamsClass_Upload s_params_Upload = new ActionParamsClass_Upload();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Upload UploadParams { get { return s_params_Upload; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Upload
        {
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_View s_params_View = new ActionParamsClass_View();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_View ViewParams { get { return s_params_View; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_View
        {
            public readonly string documentId = "documentId";
        }
        static readonly ActionParamsClass_ChangeViewPolicy s_params_ChangeViewPolicy = new ActionParamsClass_ChangeViewPolicy();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ChangeViewPolicy ChangeViewPolicyParams { get { return s_params_ChangeViewPolicy; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ChangeViewPolicy
        {
            public readonly string documentId = "documentId";
            public readonly string isShared = "isShared";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string Upload = "Upload";
                public readonly string View = "View";
            }
            public readonly string Upload = "~/Views/Documents/Upload.cshtml";
            public readonly string View = "~/Views/Documents/View.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_DocumentsController : AzureTraining.Web.Controllers.DocumentsController
    {
        public T4MVC_DocumentsController() : base(Dummy.Instance) { }

        partial void UploadOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult Upload()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Upload);
            UploadOverride(callInfo);
            return callInfo;
        }

        partial void UploadOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, AzureTraining.Web.Models.DocumentUploadViewModel model);

        public override System.Web.Mvc.ActionResult Upload(AzureTraining.Web.Models.DocumentUploadViewModel model)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Upload);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            UploadOverride(callInfo, model);
            return callInfo;
        }

        partial void ViewOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string documentId);

        public override System.Web.Mvc.ActionResult View(string documentId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.View);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "documentId", documentId);
            ViewOverride(callInfo, documentId);
            return callInfo;
        }

        partial void ChangeViewPolicyOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string documentId, bool isShared);

        public override System.Web.Mvc.ActionResult ChangeViewPolicy(string documentId, bool isShared)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChangeViewPolicy);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "documentId", documentId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "isShared", isShared);
            ChangeViewPolicyOverride(callInfo, documentId, isShared);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591

using System.IO;
using System.Web.Mvc;

namespace AzureTraining.Web.Binders
{
    class DocumentUploadViewModelModelBinder : DefaultModelBinder
    {
        public static byte[] GetBytes(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name == "Content" && controllerContext.RequestContext.HttpContext.Request.Files.Count != 0)
            {
                var requestFiles = controllerContext.RequestContext.HttpContext.Request.Files;
                if (requestFiles.Count > 0)
                {
                    var bytes = GetBytes(requestFiles[0].InputStream);
                    var content = System.Text.Encoding.Unicode.GetString(bytes);
                    SetProperty(controllerContext, bindingContext, propertyDescriptor, content);
                    return;
                }
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}

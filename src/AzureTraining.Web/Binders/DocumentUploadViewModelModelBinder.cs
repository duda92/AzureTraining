using System;
using System.IO;
using System.Linq;
using System.Web;
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
            if (propertyDescriptor.Name == "Content")
            {
                try 
                {
                    if (controllerContext.RequestContext.HttpContext.Request.Files.Count != 0)
                    {
                        var bytes = GetBytes(controllerContext.RequestContext.HttpContext.Request.Files[0].InputStream);
                        var content = System.Text.Encoding.Unicode.GetString(bytes);
                        SetProperty(controllerContext, bindingContext, propertyDescriptor, content);
                    }
                    base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
                }
                catch (HttpRequestValidationException)
                {
                    bindingContext.ModelState.AddModelError("Content", "File content doesn't looks like a simple text file");
                }
                return;
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}

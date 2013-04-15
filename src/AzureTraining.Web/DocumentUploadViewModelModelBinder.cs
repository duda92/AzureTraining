using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace AzureTraining.Web
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
                byte[] imageBytes = null;
                if (controllerContext.HttpContext.Request.Files.Count == 1 && controllerContext.HttpContext.Request.Files[0].FileName != string.Empty)
                {
                    var stream = controllerContext.HttpContext.Request.Files[0].InputStream;
                    imageBytes = GetBytes(stream);
                    var text = System.Text.Encoding.UTF8.GetString(imageBytes);
                    propertyDescriptor.SetValue(bindingContext.Model, text);
                    return;
                }
                else
                {
                    string text = bindingContext.ValueProvider.GetValue("Text").AttemptedValue;
                    propertyDescriptor.SetValue(bindingContext.Model, text);
                    return;
                }
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}

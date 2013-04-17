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
                    base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
                }
                catch (HttpRequestValidationException)
                {
 
                }
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}

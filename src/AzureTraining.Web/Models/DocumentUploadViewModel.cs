using System.ComponentModel.DataAnnotations;

namespace AzureTraining.Web.Models
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage="Set the name of file")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(50, ErrorMessage = "Maximum {50} characters exceeded")]
        public string Name { get; set; }

        public bool IsShared { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
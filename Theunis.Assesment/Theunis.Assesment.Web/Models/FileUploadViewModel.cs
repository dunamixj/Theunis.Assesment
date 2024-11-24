using System.ComponentModel.DataAnnotations;

namespace Theunis.Assesment.Web.Models
{
    public class FileUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        public IFormFile UploadedFile { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Theunis.Assesment.Web.Data.Models;

namespace Theunis.Assesment.Web.Models
{
    public class FileUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }

    }
}

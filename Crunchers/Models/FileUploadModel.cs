using System.ComponentModel.DataAnnotations;

namespace Crunchers.Models
{
    public class FileUploadModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Загрузить файл")]
        [Required(ErrorMessage = "Please choose file to upload.")]
        public string File { get; set; }
    }
}
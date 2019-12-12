using System.ComponentModel.DataAnnotations;

namespace Arshinov.WebApp.ViewModels
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string Number { get; set; }
    }
}
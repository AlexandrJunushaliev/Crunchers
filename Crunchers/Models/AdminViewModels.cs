using System.ComponentModel.DataAnnotations;

namespace Crunchers.Models
{
    public class CategoryViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 3)]
        [Display(Name = "Название категории")]
        public string NewCategoryName { get; set; }
    }
}
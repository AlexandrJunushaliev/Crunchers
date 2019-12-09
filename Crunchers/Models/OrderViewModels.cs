using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Crunchers.Models
{
    public class ValidateDateRange: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Необходимо указать дату");
            }           
            // your validation logic
            if ((DateTime)value >= DateTime.Today.Add(new TimeSpan(1,0,0,0)) && (DateTime) value <= DateTime.Today.Add(new TimeSpan(31,0,0,0)) )
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Укажите дату с {DateTime.Today.Add(new TimeSpan(1,0,0,0)).ToShortDateString()} до {DateTime.Today.Add(new TimeSpan(31,0,0,0)).ToShortDateString()}");
            }
        }
    }
    public class MakeOrderViewModel
    {
        public MakePickUpOrder MakePickUpOrder { get; set; }
        public MakeDeliverOrder MakeDeliverOrder { get; set; }
    }

    public class MakeDeliverOrder
    {
        [Required]
        [StringLength(48, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        
        [StringLength(128, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required]
        [RegularExpression(
            @"((\+7)|8)?(\-|\s)?(\d){3}(\-|\s)?(((\d){7})|((\d){3})(\-|\s)?((\d){2})(\-|\s)?((\d){2}))+",
            ErrorMessage = "Некорректный номер")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(13, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 10)]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [ValidateDateRange]
        [Display(Name = "Удобное время и дата")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime ComfortTime { get; set; }
    }

    public class MakePickUpOrder
    {
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewNewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string NewField { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewNewPassword { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Crunchers.Models
{
    public class ValidateDateRange : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Необходимо указать дату");
            }

            // your validation logic
            if ((DateTime) value >= DateTime.Today.Add(new TimeSpan(1, 0, 0, 0)) &&
                (DateTime) value <= DateTime.Today.Add(new TimeSpan(31, 0, 0, 0)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(
                    $"Укажите дату с {DateTime.Today.Add(new TimeSpan(1, 0, 0, 0)).ToShortDateString()} до {DateTime.Today.Add(new TimeSpan(31, 0, 0, 0)).ToShortDateString()}");
            }
        }
    }

    public class LessThen : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Необходимо указать время");
            }

            var propertyInfo = validationContext.ObjectType.GetProperty("ToTime");
            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (propertyValue == null)
            {
                return new ValidationResult("Необходимо указать время до");
            }

            // your validation logic
            if ((int) value < (int) propertyValue)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(
                    $"Время От должно быть меньше времени До");
            }
        }
    }

    public class MakeOrderViewModel
    {
        public MakePickUpOrder MakePickUpOrder { get; set; }
        public MakeDeliverOrder MakeDeliverOrder { get; set; }
        public IEnumerable<PointsOfPickUpModel> PointsOfPickUp { get; set; }
    }

    public class MakeDeliverOrder
    {
        [Required]
        [StringLength(48,
            MinimumLength = 2, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [DataType(DataType.Text)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [StringLength(128,
            MinimumLength = 6, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(128,
            MinimumLength = 3, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [DataType(DataType.Text)]
        [RegularExpression(
            @"(?:[У]|[у])л[.]\s?(?:\w+|\s\w+)+[,]\s?(?:[Д]|[д])[.]\s?(?:\d+\w?|\s\d+\w?)+[,]\s?(?:[К]|[к])в[.]\s?(?:\d+\w?|\s\d+\w?)+",
            ErrorMessage = "Требуемый формат - ул.(название улицы), д.(номер дома), кв.(номер кв)")]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required]
        [RegularExpression(
            @"((\+7)|8)?(\-|\s)?(\d){3}(\-|\s)?(((\d){7})|((\d){3})(\-|\s)?((\d){2})(\-|\s)?((\d){2}))+",
            ErrorMessage = "Некорректный номер")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(13,
            MinimumLength = 10, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [ValidateDateRange]
        [Display(Name = "Удобная дата")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime ComfortDate { get; set; }

        [Display(Name = "От")]
        [LessThen]
        [Range(7, 18)]
        public int FromTime { get; set; }

        [Display(Name = "До")] [Range(7, 18)] public int ToTime { get; set; }

        [StringLength(16,
            MinimumLength = 10, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [RegularExpression(
            @"\d+",
            ErrorMessage = "Некорректный номер карты")]
        [Display(Name = "Номер карты")]
        public string CardNumber { get; set; }
        public string NameRu { get; set; }
    }

    public class MakePickUpOrder
    {
        public string NameRu { get; set; }

        [Required]
        [Display(Name = "Точка вывоза")]
        public string PointOfPickUp { get; set; }

        [Required]
        [StringLength(48,
            MinimumLength = 2, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [DataType(DataType.Text)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [StringLength(128,
            MinimumLength = 6, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(
            @"((\+7)|8)?(\-|\s)?(\d){3}(\-|\s)?(((\d){7})|((\d){3})(\-|\s)?((\d){2})(\-|\s)?((\d){2}))+",
            ErrorMessage = "Некорректный номер")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(13,
            MinimumLength = 10, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [StringLength(16,
            MinimumLength = 10, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.")]
        [RegularExpression(
            @"\d+",
            ErrorMessage = "Некорректный номер карты")]
        [Display(Name = "Номер карты")]
        public string CardNumber { get; set; }
    }
}
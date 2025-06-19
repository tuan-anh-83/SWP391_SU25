using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.ValidationAttributes
{
    public class CustomFutureDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date is required!");
            }

            if (value is DateTime date)
            {
                var tomorrow = DateTime.Today.AddDays(1);
                if (date < tomorrow)
                {
                    return new ValidationResult("Date must be tomorrow or later.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

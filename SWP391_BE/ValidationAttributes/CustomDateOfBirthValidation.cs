using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.ValidationAttributes
{
    public class CustomDateOfBirthValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date of birth is required !");
            }

            if (value is DateTime dateOfBirth)
            {
                // Check if date of birth is in the future
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult("Date of birth cannot be in the future");
                }

            }

            return ValidationResult.Success;
        }
    }

   
}

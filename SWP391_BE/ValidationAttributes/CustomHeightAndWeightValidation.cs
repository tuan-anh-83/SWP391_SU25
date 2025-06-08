using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.ValidationAttributes
{
    public class CustomHeightAndWeightValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
         

            if (value is decimal decimalValue)
            {
                // Check if the property name contains "height" or "weight"
                string propertyName = validationContext.MemberName.ToLower();
                
                if (propertyName.Contains("height"))
                {
                    if (decimalValue <= 0 || decimalValue > 200)
                    {
                        return new ValidationResult("Height must be between 0 and 200 cm");
                    }
                }
                else if (propertyName.Contains("weight"))
                {
                    if (decimalValue <= 0 || decimalValue > 200)
                    {
                        return new ValidationResult("Weight must be between 0 and 200 kg");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}

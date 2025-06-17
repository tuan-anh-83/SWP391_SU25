using SWP391_BE.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class PartialAccountUpdateRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "Fullname must be up to 50 characters long.")]
        public string? Fullname { get; set; }

        [StringLength(50, ErrorMessage = "Address must be up to 50 characters long.")]
        public string? Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [CustomPhoneValidation]
        public string? PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}

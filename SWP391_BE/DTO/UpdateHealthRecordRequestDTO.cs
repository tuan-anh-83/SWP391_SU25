using SWP391_BE.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class UpdateHealthRecordRequestDTO
    {
        [Required(ErrorMessage = "ParentId is required.")]
        public int HealthRecordId { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required.")]
        [CustomDateOfBirthValidation]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }
        [Required(ErrorMessage = "Height is required.")]
        [CustomHeightAndWeightValidation]
        public double Height { get; set; }
        [Required(ErrorMessage = "Weight is required.")]
        [CustomHeightAndWeightValidation]
        public double Weight { get; set; }
    }
} 
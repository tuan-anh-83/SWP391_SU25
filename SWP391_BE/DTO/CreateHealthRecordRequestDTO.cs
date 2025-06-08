using SWP391_BE.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class CreateHealthRecordRequestDTO
    {
        [Required(ErrorMessage = "ParentId is required.")]
        public int ParentId { get; set; }
        [Required(ErrorMessage = "StudentId is required.")]
        public int StudentId { get; set; }
        [Required(ErrorMessage = "StudentName is required.")]
        public string StudentName { get; set; }
        [Required(ErrorMessage = "StudentCode is required.")]
        public string StudentCode { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required.")]
        [CustomDateOfBirthValidation]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }
        [Required(ErrorMessage = "Height is required.")]
        public double Height { get; set; }
        [Required(ErrorMessage = "Weight is required.")]
        public double Weight { get; set; }
    }
} 
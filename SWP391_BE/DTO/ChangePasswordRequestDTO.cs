﻿using SWP391_BE.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class ChangePasswordRequestDTO
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [CustomPasswordValidation]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm new password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

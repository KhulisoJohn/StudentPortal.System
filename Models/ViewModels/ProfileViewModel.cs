using System;
using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Enums;

namespace StudentPortalSystem.Models.ViewModels
{
    public class ProfileViewModel
    {
        public DateTime CreatedAt { get; set; }

        // Detect roles explicitly using enum instead of flags
        public UserRole Role { get; set; }

        public bool IsStudent => Role == UserRole.Student;
        public bool IsTutor => Role == UserRole.Tutor;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name must be less than 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class ProfileViewModel
    {
        public DateTime CreatedAt { get; set; }
        public bool IsStudent { get; set; }
        public bool IsTutor { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

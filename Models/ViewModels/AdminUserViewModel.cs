using StudentPortalSystem.Enums;
using System.ComponentModel.DataAnnotations;
namespace StudentPortalSystem.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
         [Required]
        public UserRole Role { get; set; } = UserRole.Student;

        public bool IsActive { get; set; }  // optional, for locking users
    }
}


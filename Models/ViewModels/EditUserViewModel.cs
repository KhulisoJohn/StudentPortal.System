using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Enums;

namespace StudentPortalSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        // Instead of multiple roles, use a single strongly typed UserRole
        [Required]
        public UserRole Role { get; set; }

        // For the dropdown list in Razor view
        public List<UserRole> AvailableRoles { get; set; } = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
        public string PhoneNumber { get; internal set; } = string.Empty;
        public object SelectedRole { get; internal set; }
        public List<string> AllRoles { get; internal set; } 
    }
}

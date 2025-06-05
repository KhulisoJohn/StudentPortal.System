using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models.ViewModels
{
   public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = null!; // Non-null by design

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public List<string> SelectedRoles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
    } 
}


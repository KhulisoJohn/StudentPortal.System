using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class UserProfile
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    public Student? Student { get; set; } // Optional 1-to-1
}
}



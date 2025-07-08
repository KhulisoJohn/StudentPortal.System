using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Enums;

public class ApplicationUserDto
{
    // The unique user identifier assigned by the framework (read-only)
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}

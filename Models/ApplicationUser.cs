using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        // Added to determine user role explicitly
        [Required]
        public string? Role { get; set; }  // "Student" or "Tutor"

        // Navigation properties (1-to-1)
        public Student? Student { get; set; }
        public Tutor? Tutor { get; set; }

        // Chat system
        public ICollection<UserChatChannel>? UserChatChannels { get; set; }
        public ICollection<ChatMessage>? ChatMessages { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

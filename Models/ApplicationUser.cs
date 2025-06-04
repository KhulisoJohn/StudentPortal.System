using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using StudentPortal.Models;

namespace StudentPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        // One-to-one relationships (nullable at first)
        public Student? StudentProfile { get; set; }
        public Tutor? TutorProfile { get; set; }

         public ICollection<UserChatChannel>? UserChatChannels { get; set; }
    public ICollection<ChatMessage>? ChatMessages { get; set; }

        // Optional: Time of registration
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

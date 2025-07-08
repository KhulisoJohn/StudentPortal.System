using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Chat;
using StudentPortalSystem.Enums;
using System.Collections.Generic;

namespace StudentPortalSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Student;
        public UserStatus Status { get; set; } = UserStatus.Pending;

        // Navigation properties
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? TutorId { get; set; }
        public Tutor? Tutor { get; set; }

        // Collections
        public virtual ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public virtual ICollection<UserChatChannel> UserChatChannels { get; set; } = new List<UserChatChannel>();
        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        // Timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentPortalSystem.Enums;

namespace StudentPortalSystem.Models
{
    public class Tutor
    {
        [Key]
        public int Id { get; set; }

        // Link to Identity user
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }

        // Status of the tutor account (e.g., Pending, Active, Blocked)
        [Required]
        public UserStatus Status { get; set; } = UserStatus.Pending;

        // The grade range the tutor teaches (Grade 4–9 or Grade 10–12)
        [Required]
        public GradeRange GradeRange { get; set; }

        // Short biography or description
        [StringLength(500)]
        public string? Bio { get; set; }

        // Optional contact info beyond ApplicationUser.PhoneNumber
        [StringLength(100)]
        public string? ContactInfo { get; set; }

        // Date when tutor was officially hired or joined
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        // Auto-generated timestamp for record creation
        [DataType(DataType.DateTime)]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Whether the tutor is currently active in the system
        public bool IsActive { get; set; } = true;

        // Navigation: subjects the tutor teaches (max 4 enforced in logic)
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Tutor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        // Subjects this tutor teaches
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();

        public string? Bio { get; set; }

        public string? ContactInfo { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}

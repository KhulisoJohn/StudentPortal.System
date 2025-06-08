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
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        // Subjects this tutor teaches (many-to-many)
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();

        public string? Bio { get; set; }
        
        public DateTime HireDate { get; set; } = DateTime.UtcNow;


        public string? ContactInfo { get; set; }

        // Optional fields that may help later
        public bool IsActive { get; set; } = true;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
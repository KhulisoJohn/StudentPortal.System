using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortalSystem.Models
{
    public class Tutor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? ContactInfo { get; set; }

         public DateTime HireDate { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
       

        public bool IsActive { get; set; } = true;
    }
}

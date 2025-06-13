using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class TutorSubject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TutorId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(TutorId))]
        public Tutor? Tutor { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject? Subject { get; set; }

        public bool Approved { get; set; } = false;

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
    }
}

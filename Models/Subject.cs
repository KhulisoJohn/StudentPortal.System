using System.Collections.Generic;
using StudentPortalSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortalSystem.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // Add string length for validation
        public string Name { get; set; } = string.Empty; // Avoid nullable here

        // Grade that subject belongs to (Grade 10–12 only)
        [Range(10, 12)]
        public int Grade { get; set; }

        // Navigation properties
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();

        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
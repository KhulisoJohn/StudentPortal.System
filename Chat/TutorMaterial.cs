using System;
using StudentPortalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalSystem.Chat
{
    public class TutorMaterial
    {
        public int Id { get; set; }
        
        [Required]
        public string? Title { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public string? FilePath { get; set; } // Or URL if using cloud storage
        
        public DateTime UploadDate { get; set; }
        
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
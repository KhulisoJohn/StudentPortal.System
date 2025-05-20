using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string? Title { get; set; }

        // Foreign key to Author (required)
        [Required(ErrorMessage = "Author is required")]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }

        // Foreign key to Course (optional)
        public int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}

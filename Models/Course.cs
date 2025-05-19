using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Title { get; set; }

    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
}


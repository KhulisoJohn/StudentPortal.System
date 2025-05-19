using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string? Title { get; set; }

    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

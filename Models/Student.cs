using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentPortal.Models;

namespace StudentPortal.Models
{
    public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;

    [ForeignKey("ApplicationUserId")]
    public ApplicationUser? ApplicationUser { get; set; }

    public ICollection<StudentSubject>? StudentSubjects { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
}


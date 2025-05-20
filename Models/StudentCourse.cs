namespace StudentPortal.Models
{
   public class StudentCourse
{
    public int Id { get; set; }  // 👈 Add this!
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }
} 
}


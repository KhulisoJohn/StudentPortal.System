public class Course
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public ICollection<StudentCourse> StudentCourses { get; set; }
    public ICollection<Book> Books { get; set; }
}

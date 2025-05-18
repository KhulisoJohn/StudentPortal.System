public class Student
{
    public int Id { get; set; }
    public string? FullName { get; set; }

    public int? UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public ICollection<StudentCourse>StudentCourses { get; set; }
}

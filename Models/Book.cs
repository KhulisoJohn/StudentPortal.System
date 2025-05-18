public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

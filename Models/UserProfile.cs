public class UserProfile
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }

    public Student? Student { get; set; } // Optional 1-to-1
}

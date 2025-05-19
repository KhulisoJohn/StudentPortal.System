using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? FullName { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}


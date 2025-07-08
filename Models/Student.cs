using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentPortalSystem.Enums;

namespace StudentPortalSystem.Models
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

        public UserStatus Status { get; set; } = UserStatus.Pending;

        [Required]
        public GradeRange Grade { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public bool CanJoinSubjectChannels { get; set; } = false;
    }
}

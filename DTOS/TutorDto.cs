using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Enums;

namespace StudentPortalSystem.DTOs
{
    public class TutorEnrollmentDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [Required]
        public GradeRange GradeRange { get; set; }

        [Required]
        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;

        [Phone]
        [StringLength(100)]
        public string? ContactInfo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        // Optional — useful for admin scenarios
        public UserStatus Status { get; set; } = UserStatus.Pending;

        [Required]
        [MinLength(1)]
        [MaxLength(4, ErrorMessage = "You can only select up to 4 subjects.")]
        public List<int> SelectedSubjectIds { get; set; } = new();
    }
}

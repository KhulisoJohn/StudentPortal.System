using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentPortalSystem.Enums;

public class StudentEnrollmentDto
{
    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    public GradeRange Grade { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Phone]
    [Required]
    public string EmegencyContactNumber { get; set; } = string.Empty;

    // Parent/Guardian details
    [Required]
    [StringLength(100)]
    public string ParentFullName { get; set; } = string.Empty;

    [Phone]
    [Required]
    public string ParentContactNumber { get; set; } = string.Empty;

    // Only applicable if Grade 10+
    // Require exactly 4 subjects
    // Use Subject enum or subject IDs depending on your data model
    public List<Subjects> SelectedSubjectIds { get; set; } = new List<Subjects>();


    // This flag can be set server-side during enrollment based on grade
    public bool CanJoinSubjectChannels { get; set; } = false;
}


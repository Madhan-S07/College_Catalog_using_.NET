using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    // What the frontend sends for Add and Edit
    public class EnrollmentRequestDTO
    {
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        // leave empty until a grade is given
        [Range(0, 100)]
        public int? Grade { get; set; }
    }

    // What the API sends back to the frontend
    public class EnrollmentResponseDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrolledOn { get; set; }
        public int? Grade { get; set; }
    }
}
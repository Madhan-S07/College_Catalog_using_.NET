using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    // What the frontend sends for Add and Edit
    public class CourseRequestDTO
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Credits { get; set; }

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }

    // What the API sends back to the frontend
    public class CourseResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
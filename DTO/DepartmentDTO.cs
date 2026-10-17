using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    // What the frontend sends for Add and Edit
    public class DepartmentRequestDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CollegeId { get; set; }
    }

    // What the API sends back to the frontend
    public class DepartmentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;
    }
}
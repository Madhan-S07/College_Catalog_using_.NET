using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    // What the frontend sends for Add and Edit
    public class StudentRequestDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }

    // What the API sends back to the frontend
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
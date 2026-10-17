using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.DTO 
{ 
    public class LoginRequestDTO 
    { 
        [Required]
        public string user { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    } 
}
using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}
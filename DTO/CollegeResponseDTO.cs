using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApi.Models
{
    public class CollegeResponseDTO
    {
        public int College_Id {get; set;}

        [Required]
        [MaxLength(100)]
        public string Name {get; set;} = string.Empty; 

        [Required]
        [MaxLength(100)]
        public string Dept {get; set;} = string.Empty;

        [Range(1, 100000)]
        public int No_Students {get; set;}
    }
}
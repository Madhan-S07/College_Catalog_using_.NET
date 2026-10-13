namespace CollegeManagementApi.Models
{
    
    public class CollegeResponseDTO
    {
        public int College_Id {get; set;}
        public string Name {get; set;} = string.Empty; 
        public string Dept {get; set;} = string.Empty;
        public int No_Students {get; set;}
    }
}
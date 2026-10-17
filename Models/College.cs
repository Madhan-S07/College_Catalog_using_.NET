namespace CollegeManagementApi.Models
{
    
    public class College
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty; 
        public string Dept {get; set;} = string.Empty;
        public int NO_Students{get; set;}
        public List<Department> Departments { get; set; } = new();
    }
}

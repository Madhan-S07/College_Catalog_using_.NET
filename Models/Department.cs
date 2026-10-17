namespace CollegeManagementApi.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CollegeId { get; set; }
        public College? College { get; set; }

        public List<Student> Students { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
    }
}
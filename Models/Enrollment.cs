namespace CollegeManagementApi.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
        public int? Grade { get; set; }   // empty until a grade is given
    }
}
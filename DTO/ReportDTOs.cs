namespace CollegeManagementApi.DTO
{
    public class DepartmentSummaryDTO
    {
        public string College { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int CourseCount { get; set; }
    }

    public class StudentCourseDTO
    {
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int? Grade { get; set; }
    }

    public class StudentDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public double? AverageGrade { get; set; }
        public List<StudentCourseDTO> Courses { get; set; } = new();
    }

    public class TopCollegeDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Dept { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }

    public class CourseStatDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int EnrolledCount { get; set; }
        public double? AverageGrade { get; set; }
    }

    public class StudentListItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class PagedResultDTO<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
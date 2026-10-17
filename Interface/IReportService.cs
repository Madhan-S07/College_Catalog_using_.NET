using CollegeManagementApi.DTO;

namespace CollegeManagementApi.Interface
{
    public interface IReportService
    {
        Task<List<DepartmentSummaryDTO>> GetDepartmentSummary();
        Task<StudentDetailDTO?> GetStudentDetail(int id);
        Task<List<TopCollegeDTO>> GetTopColleges(int count);
        Task<List<CourseStatDTO>> GetCourseStats();
        Task<PagedResultDTO<StudentListItemDTO>> SearchStudents(string? search, int? departmentId, int page, int pageSize);
    }
}
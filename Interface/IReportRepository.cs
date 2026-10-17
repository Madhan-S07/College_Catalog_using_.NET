using CollegeManagementApi.DTO;

namespace CollegeManagementApi.Interface
{
    public interface IReportRepository
    {
        Task<List<DepartmentSummaryDTO>> GetDepartmentSummaryAsync();
        Task<StudentDetailDTO?> GetStudentDetailAsync(int id);
        Task<List<TopCollegeDTO>> GetTopCollegesAsync(int count);
        Task<List<CourseStatDTO>> GetCourseStatsAsync();
        Task<PagedResultDTO<StudentListItemDTO>> SearchStudentsAsync(string? search, int? departmentId, int page, int pageSize);
    }
}
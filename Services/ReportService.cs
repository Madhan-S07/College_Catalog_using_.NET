using CollegeManagementApi.DTO;
using CollegeManagementApi.Interface;

namespace CollegeManagementApi.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository repo, ILogger<ReportService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<DepartmentSummaryDTO>> GetDepartmentSummary()
        {
            _logger.LogInformation("Building department summary");
            return await _repo.GetDepartmentSummaryAsync();
        }

        public async Task<StudentDetailDTO?> GetStudentDetail(int id)
        {
            _logger.LogInformation("Getting details for student {Id}", id);
            return await _repo.GetStudentDetailAsync(id);
        }

        public async Task<List<TopCollegeDTO>> GetTopColleges(int count)
        {
            if (count < 1) count = 1;
            if (count > 50) count = 50;
            return await _repo.GetTopCollegesAsync(count);
        }

        public async Task<List<CourseStatDTO>> GetCourseStats()
        {
            return await _repo.GetCourseStatsAsync();
        }

        public async Task<PagedResultDTO<StudentListItemDTO>> SearchStudents(
            string? search, int? departmentId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            _logger.LogInformation("Searching students: search={Search}, department={Dept}, page={Page}",
                search, departmentId, page);

            return await _repo.SearchStudentsAsync(search, departmentId, page, pageSize);
        }
    }
}
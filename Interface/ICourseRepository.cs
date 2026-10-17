using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<Course>> GetAllWithDepartmentAsync();
        Task<Course?> GetByIdWithDepartmentAsync(int id);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<bool> TitleExistsAsync(string title, int departmentId, int? excludeCourseId = null);
    }
}
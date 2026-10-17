using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<List<Department>> GetAllWithCollegeAsync();
        Task<Department?> GetByIdWithCollegeAsync(int id);
        Task<bool> CollegeExistsAsync(int collegeId);
        Task<bool> NameExistsAsync(string name, int collegeId, int? excludeDepartmentId = null);
    }
}
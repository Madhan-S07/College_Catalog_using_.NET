using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<List<Student>> GetAllWithDepartmentAsync();
        Task<Student?> GetByIdWithDepartmentAsync(int id);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null);
    }
}
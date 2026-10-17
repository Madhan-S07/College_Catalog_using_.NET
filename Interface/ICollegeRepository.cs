using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface ICollegeRepository : IRepository<College>
    {
        Task<College?> FindByNameAndDeptAsync(string name, string dept);
    }
}
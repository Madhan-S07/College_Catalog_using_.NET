using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Data
{
    public class CollegeRepository : Repository<College>, ICollegeRepository
    {
        public CollegeRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<College?> FindByNameAndDeptAsync(string name, string dept)
        {
            var nameKey = name.ToLower();
            var deptKey = dept.ToLower();

            return await _db.StudentsDetails
                .FirstOrDefaultAsync(c => c.Name.ToLower() == nameKey && c.Dept.ToLower() == deptKey);
        }
    }
}
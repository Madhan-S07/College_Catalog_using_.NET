using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Data
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Department>> GetAllWithCollegeAsync()
        {
            return await _db.Departments
                .Include(d => d.College)
                .OrderBy(d => d.Id)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdWithCollegeAsync(int id)
        {
            return await _db.Departments
                .Include(d => d.College)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CollegeExistsAsync(int collegeId)
        {
            return await _db.StudentsDetails.AnyAsync(c => c.Id == collegeId);
        }

        // the same department name is allowed in different colleges, but not twice in one college
        public async Task<bool> NameExistsAsync(string name, int collegeId, int? excludeDepartmentId = null)
        {
            var key = name.ToLower();
            return await _db.Departments.AnyAsync(d =>
                d.CollegeId == collegeId &&
                d.Name.ToLower() == key &&
                (excludeDepartmentId == null || d.Id != excludeDepartmentId));
        }
    }
}
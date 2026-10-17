using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Data
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Course>> GetAllWithDepartmentAsync()
        {
            return await _db.Courses
                .Include(c => c.Department)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithDepartmentAsync(int id)
        {
            return await _db.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _db.Departments.AnyAsync(d => d.Id == departmentId);
        }

        // the same course title is allowed in different departments, but not twice in one department
        public async Task<bool> TitleExistsAsync(string title, int departmentId, int? excludeCourseId = null)
        {
            var key = title.ToLower();
            return await _db.Courses.AnyAsync(c =>
                c.DepartmentId == departmentId &&
                c.Title.ToLower() == key &&
                (excludeCourseId == null || c.Id != excludeCourseId));
        }
    }
}
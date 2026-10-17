using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Data
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Student>> GetAllWithDepartmentAsync()
        {
            return await _db.Students
                .Include(s => s.Department)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdWithDepartmentAsync(int id)
        {
            return await _db.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _db.Departments.AnyAsync(d => d.Id == departmentId);
        }

        // excludeStudentId lets a student keep their own email when editing
        public async Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null)
        {
            var key = email.ToLower();
            return await _db.Students.AnyAsync(s =>
                s.Email.ToLower() == key && (excludeStudentId == null || s.Id != excludeStudentId));
        }
    }
}
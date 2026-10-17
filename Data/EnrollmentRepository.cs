using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Data
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Enrollment>> GetAllWithDetailsAsync()
        {
            return await _db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderBy(e => e.Id)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        {
            return await _db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> StudentExistsAsync(int studentId)
        {
            return await _db.Students.AnyAsync(s => s.Id == studentId);
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _db.Courses.AnyAsync(c => c.Id == courseId);
        }

        // a student can enroll in the same course only once (same rule as the unique index)
        public async Task<bool> AlreadyEnrolledAsync(int studentId, int courseId, int? excludeEnrollmentId = null)
        {
            return await _db.Enrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                (excludeEnrollmentId == null || e.Id != excludeEnrollmentId));
        }
    }
}
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<List<Enrollment>> GetAllWithDetailsAsync();
        Task<Enrollment?> GetByIdWithDetailsAsync(int id);
        Task<bool> StudentExistsAsync(int studentId);
        Task<bool> CourseExistsAsync(int courseId);
        Task<bool> AlreadyEnrolledAsync(int studentId, int courseId, int? excludeEnrollmentId = null);
    }
}
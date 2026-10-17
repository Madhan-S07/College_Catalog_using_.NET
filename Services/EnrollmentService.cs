using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(IEnrollmentRepository repo, ILogger<EnrollmentService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<EnrollmentResponseDTO>> GetAll()
        {
            _logger.LogInformation("Getting all enrollments");
            var enrollments = await _repo.GetAllWithDetailsAsync();
            return enrollments.Select(ToDto).ToList();
        }

        public async Task<EnrollmentResponseDTO?> GetById(int id)
        {
            var enrollment = await _repo.GetByIdWithDetailsAsync(id);
            return enrollment == null ? null : ToDto(enrollment);
        }

        public async Task<(EnrollmentResponseDTO? Enrollment, string? Error)> Add(EnrollmentRequestDTO dto)
        {
            if (!await _repo.StudentExistsAsync(dto.StudentId))
                return (null, "Student not found");

            if (!await _repo.CourseExistsAsync(dto.CourseId))
                return (null, "Course not found");

            if (await _repo.AlreadyEnrolledAsync(dto.StudentId, dto.CourseId))
                return (null, "This student is already enrolled in this course");

            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Grade = dto.Grade,
                EnrolledOn = DateTime.UtcNow
            };

            await _repo.AddAsync(enrollment);
            await _repo.SaveAsync();
            _logger.LogInformation("Enrolled student {StudentId} in course {CourseId}",
                dto.StudentId, dto.CourseId);

            var saved = await _repo.GetByIdWithDetailsAsync(enrollment.Id);
            return (ToDto(saved!), null);
        }

        public async Task<(EnrollmentResponseDTO? Enrollment, string? Error)> Update(int id, EnrollmentRequestDTO dto)
        {
            var enrollment = await _repo.GetByIdWithDetailsAsync(id);
            if (enrollment == null)
                return (null, "Enrollment not found");

            if (!await _repo.StudentExistsAsync(dto.StudentId))
                return (null, "Student not found");

            if (!await _repo.CourseExistsAsync(dto.CourseId))
                return (null, "Course not found");

            if (await _repo.AlreadyEnrolledAsync(dto.StudentId, dto.CourseId, id))
                return (null, "This student is already enrolled in this course");

            enrollment.StudentId = dto.StudentId;
            enrollment.CourseId = dto.CourseId;
            enrollment.Grade = dto.Grade;

            await _repo.SaveAsync();
            _logger.LogInformation("Updated enrollment {Id}", id);

            var saved = await _repo.GetByIdWithDetailsAsync(id);
            return (ToDto(saved!), null);
        }

        public async Task<bool> Delete(int id)
        {
            var enrollment = await _repo.GetByIdAsync(id);
            if (enrollment == null)
            {
                _logger.LogWarning("Delete failed: enrollment {Id} not found", id);
                return false;
            }

            _repo.Remove(enrollment);
            await _repo.SaveAsync();
            _logger.LogInformation("Deleted enrollment {Id}", id);
            return true;
        }

        private static EnrollmentResponseDTO ToDto(Enrollment e)
        {
            return new EnrollmentResponseDTO
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student?.Name ?? string.Empty,
                CourseId = e.CourseId,
                CourseTitle = e.Course?.Title ?? string.Empty,
                EnrolledOn = e.EnrolledOn,
                Grade = e.Grade
            };
        }
    }
}
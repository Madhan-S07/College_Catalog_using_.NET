using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository repo, ILogger<CourseService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<CourseResponseDTO>> GetAll()
        {
            _logger.LogInformation("Getting all courses");
            var courses = await _repo.GetAllWithDepartmentAsync();
            return courses.Select(ToDto).ToList();
        }

        public async Task<CourseResponseDTO?> GetById(int id)
        {
            var course = await _repo.GetByIdWithDepartmentAsync(id);
            return course == null ? null : ToDto(course);
        }

        public async Task<(CourseResponseDTO? Course, string? Error)> Add(CourseRequestDTO dto)
        {
            var title = dto.Title.Trim();

            if (!await _repo.DepartmentExistsAsync(dto.DepartmentId))
                return (null, "Department not found");

            if (await _repo.TitleExistsAsync(title, dto.DepartmentId))
                return (null, "This department already has a course with this title");

            var course = new Course
            {
                Title = title,
                Credits = dto.Credits,
                DepartmentId = dto.DepartmentId
            };

            await _repo.AddAsync(course);
            await _repo.SaveAsync();
            _logger.LogInformation("Added course {Id} ({Title})", course.Id, course.Title);

            var saved = await _repo.GetByIdWithDepartmentAsync(course.Id);
            return (ToDto(saved!), null);
        }

        public async Task<(CourseResponseDTO? Course, string? Error)> Update(int id, CourseRequestDTO dto)
        {
            var course = await _repo.GetByIdWithDepartmentAsync(id);
            if (course == null)
                return (null, "Course not found");

            var title = dto.Title.Trim();

            if (!await _repo.DepartmentExistsAsync(dto.DepartmentId))
                return (null, "Department not found");

            if (await _repo.TitleExistsAsync(title, dto.DepartmentId, id))
                return (null, "This department already has a course with this title");

            course.Title = title;
            course.Credits = dto.Credits;
            course.DepartmentId = dto.DepartmentId;

            await _repo.SaveAsync();
            _logger.LogInformation("Updated course {Id} ({Title})", id, title);

            var saved = await _repo.GetByIdWithDepartmentAsync(id);
            return (ToDto(saved!), null);
        }

        public async Task<bool> Delete(int id)
        {
            var course = await _repo.GetByIdAsync(id);
            if (course == null)
            {
                _logger.LogWarning("Delete failed: course {Id} not found", id);
                return false;
            }

            _repo.Remove(course);
            await _repo.SaveAsync();
            _logger.LogInformation("Deleted course {Id} ({Title})", id, course.Title);
            return true;
        }

        private static CourseResponseDTO ToDto(Course c)
        {
            return new CourseResponseDTO
            {
                Id = c.Id,
                Title = c.Title,
                Credits = c.Credits,
                DepartmentId = c.DepartmentId,
                DepartmentName = c.Department?.Name ?? string.Empty
            };
        }
    }
}
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository repo, ILogger<StudentService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<StudentResponseDTO>> GetAll()
        {
            _logger.LogInformation("Getting all students");
            var students = await _repo.GetAllWithDepartmentAsync();
            return students.Select(ToDto).ToList();
        }

        public async Task<StudentResponseDTO?> GetById(int id)
        {
            var student = await _repo.GetByIdWithDepartmentAsync(id);
            return student == null ? null : ToDto(student);
        }

        public async Task<(StudentResponseDTO? Student, string? Error)> Add(StudentRequestDTO dto)
        {
            var name = dto.Name.Trim();
            var email = dto.Email.Trim();

            if (!await _repo.DepartmentExistsAsync(dto.DepartmentId))
                return (null, "Department not found");

            if (await _repo.EmailExistsAsync(email))
                return (null, "A student with this email already exists");

            var student = new Student
            {
                Name = name,
                Email = email,
                DepartmentId = dto.DepartmentId
            };

            await _repo.AddAsync(student);
            await _repo.SaveAsync();
            _logger.LogInformation("Added student {Id} ({Name})", student.Id, student.Name);

            var saved = await _repo.GetByIdWithDepartmentAsync(student.Id);
            return (ToDto(saved!), null);
        }

        public async Task<(StudentResponseDTO? Student, string? Error)> Update(int id, StudentRequestDTO dto)
        {
            var student = await _repo.GetByIdWithDepartmentAsync(id);
            if (student == null)
                return (null, "Student not found");

            var name = dto.Name.Trim();
            var email = dto.Email.Trim();

            if (!await _repo.DepartmentExistsAsync(dto.DepartmentId))
                return (null, "Department not found");

            if (await _repo.EmailExistsAsync(email, id))
                return (null, "A student with this email already exists");

            student.Name = name;
            student.Email = email;
            student.DepartmentId = dto.DepartmentId;

            await _repo.SaveAsync();
            _logger.LogInformation("Updated student {Id} ({Name})", id, name);

            var saved = await _repo.GetByIdWithDepartmentAsync(id);
            return (ToDto(saved!), null);
        }

        public async Task<bool> Delete(int id)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Delete failed: student {Id} not found", id);
                return false;
            }

            _repo.Remove(student);
            await _repo.SaveAsync();
            _logger.LogInformation("Deleted student {Id} ({Name})", id, student.Name);
            return true;
        }

        private static StudentResponseDTO ToDto(Student s)
        {
            return new StudentResponseDTO
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department?.Name ?? string.Empty
            };
        }
    }
}
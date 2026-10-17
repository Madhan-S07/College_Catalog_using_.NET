using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository repo, ILogger<DepartmentService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<DepartmentResponseDTO>> GetAll()
        {
            _logger.LogInformation("Getting all departments");
            var departments = await _repo.GetAllWithCollegeAsync();
            return departments.Select(ToDto).ToList();
        }

        public async Task<DepartmentResponseDTO?> GetById(int id)
        {
            var department = await _repo.GetByIdWithCollegeAsync(id);
            return department == null ? null : ToDto(department);
        }

        public async Task<(DepartmentResponseDTO? Department, string? Error)> Add(DepartmentRequestDTO dto)
        {
            var name = dto.Name.Trim();

            if (!await _repo.CollegeExistsAsync(dto.CollegeId))
                return (null, "College not found");

            if (await _repo.NameExistsAsync(name, dto.CollegeId))
                return (null, "This college already has a department with this name");

            var department = new Department
            {
                Name = name,
                CollegeId = dto.CollegeId
            };

            await _repo.AddAsync(department);
            await _repo.SaveAsync();
            _logger.LogInformation("Added department {Id} ({Name})", department.Id, department.Name);

            var saved = await _repo.GetByIdWithCollegeAsync(department.Id);
            return (ToDto(saved!), null);
        }

        public async Task<(DepartmentResponseDTO? Department, string? Error)> Update(int id, DepartmentRequestDTO dto)
        {
            var department = await _repo.GetByIdWithCollegeAsync(id);
            if (department == null)
                return (null, "Department not found");

            var name = dto.Name.Trim();

            if (!await _repo.CollegeExistsAsync(dto.CollegeId))
                return (null, "College not found");

            if (await _repo.NameExistsAsync(name, dto.CollegeId, id))
                return (null, "This college already has a department with this name");

            department.Name = name;
            department.CollegeId = dto.CollegeId;

            await _repo.SaveAsync();
            _logger.LogInformation("Updated department {Id} ({Name})", id, name);

            var saved = await _repo.GetByIdWithCollegeAsync(id);
            return (ToDto(saved!), null);
        }

        public async Task<bool> Delete(int id)
        {
            var department = await _repo.GetByIdAsync(id);
            if (department == null)
            {
                _logger.LogWarning("Delete failed: department {Id} not found", id);
                return false;
            }

            _repo.Remove(department);
            await _repo.SaveAsync();
            _logger.LogInformation("Deleted department {Id} ({Name})", id, department.Name);
            return true;
        }

        private static DepartmentResponseDTO ToDto(Department d)
        {
            return new DepartmentResponseDTO
            {
                Id = d.Id,
                Name = d.Name,
                CollegeId = d.CollegeId,
                CollegeName = d.College?.Name ?? string.Empty
            };
        }
    }
}
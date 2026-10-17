using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class CollegeService : ICollegeService
    {
        private readonly ICollegeRepository _repo;
        private readonly ILogger<CollegeService> _logger;

        public CollegeService(ICollegeRepository repo, ILogger<CollegeService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<CollegeResponseDTO>> GetAllStudents()
        {
            _logger.LogInformation("Getting all students");
            var colleges = await _repo.GetAllAsync();

            return colleges.Select(s => new CollegeResponseDTO
            {
                College_Id = s.Id,
                Name = s.Name,
                Dept = s.Dept,
                No_Students = s.NO_Students
            }).ToList();
        }

        public async Task<CollegeResponseDTO?> GetStudentById(int id)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null) return null;

            return new CollegeResponseDTO
            {
                College_Id = student.Id,
                Name = student.Name,
                Dept = student.Dept,
                No_Students = student.NO_Students
            };
        }

        // POST: adds a new college, or adds to the count if it already exists
        public async Task<bool> AddStudent(CollegeResponseDTO student)
        {
            // Trim so "MADHAN " and "madhan" match the same row
            var name = student.Name.Trim();
            var dept = student.Dept.Trim();

            var existing = await _repo.FindByNameAndDeptAsync(name, dept);

            if (existing != null)
            {
                existing.NO_Students += student.No_Students;
                _logger.LogInformation("Updated {Name} - {Dept}, new count {Count}",
                    existing.Name, existing.Dept, existing.NO_Students);
            }
            else
            {
                await _repo.AddAsync(new College
                {
                    Name = name,
                    Dept = dept,
                    NO_Students = student.No_Students
                });
            }

            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteStudent(int id)
        {
            var stu = await _repo.GetByIdAsync(id);
            if (stu == null) return false;

            _repo.Remove(stu);
            await _repo.SaveAsync();
            return true;
        }
    }
}
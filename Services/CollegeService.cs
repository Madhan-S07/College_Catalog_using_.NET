using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Data;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Services
{
    public class CollegeService : ICollegeService
    {
        private readonly AppDbContext Db;
        private readonly ILogger<CollegeService> _logger;

        public CollegeService(AppDbContext _Db, ILogger<CollegeService> logger)
        {
            Db = _Db;
            _logger = logger;
        }

        public async Task<List<CollegeResponseDTO>> GetAllStudents()
        {
            _logger.LogInformation("Getting all students");
            return await Db.StudentsDetails
                .Select(s => new CollegeResponseDTO
                {
                    College_Id = s.Id,
                    Name = s.Name,
                    Dept = s.Dept,
                    No_Students = s.NO_Students
                }).ToListAsync();
        }

        public async Task<CollegeResponseDTO?> GetStudentById(int id)
        {
            var student = await Db.StudentsDetails.FindAsync(id);
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
            // Trim and lower-case so "MADHAN " and "madhan" match the same row
            var name = student.Name.Trim();
            var dept = student.Dept.Trim();
            var nameKey = name.ToLower();
            var deptKey = dept.ToLower();

            var existing = await Db.StudentsDetails
                .FirstOrDefaultAsync(c => c.Name.ToLower() == nameKey && c.Dept.ToLower() == deptKey);

            if (existing != null)
            {
                existing.NO_Students += student.No_Students;
                _logger.LogInformation("Updated {Name} - {Dept}, new count {Count}",
                    existing.Name, existing.Dept, existing.NO_Students);
            }
            else
            {
                Db.StudentsDetails.Add(new College
                {
                    Name = name,
                    Dept = dept,
                    NO_Students = student.No_Students
                });
            }

            await Db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStudent(int id)
        {
            var stu = await Db.StudentsDetails.FindAsync(id);
            if (stu == null) return false;

            Db.StudentsDetails.Remove(stu);
            await Db.SaveChangesAsync();
            return true;
        }
    }
}
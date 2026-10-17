using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.DTO;
using CollegeManagementApi.Interface;

namespace CollegeManagementApi.Data
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _db;

        public ReportRepository(AppDbContext db)
        {
            _db = db;
        }

        // Navigation properties + counts + ordering
        public async Task<List<DepartmentSummaryDTO>> GetDepartmentSummaryAsync()
        {
            return await _db.Departments
                .OrderBy(d => d.College!.Name)
                .ThenByDescending(d => d.Students.Count)
                .Select(d => new DepartmentSummaryDTO
                {
                    College = d.College!.Name,
                    Department = d.Name,
                    StudentCount = d.Students.Count,
                    CourseCount = d.Courses.Count
                })
                .ToListAsync();
        }

        // Include / ThenInclude across 3 levels
        public async Task<StudentDetailDTO?> GetStudentDetailAsync(int id)
        {
            var student = await _db.Students
                .Include(s => s.Department).ThenInclude(d => d!.College)
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return null;

            var graded = student.Enrollments.Where(e => e.Grade != null).ToList();

            return new StudentDetailDTO
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Department = student.Department!.Name,
                College = student.Department.College!.Name,
                AverageGrade = graded.Count > 0 ? graded.Average(e => e.Grade!.Value) : (double?)null,
                Courses = student.Enrollments.Select(e => new StudentCourseDTO
                {
                    Title = e.Course!.Title,
                    Credits = e.Course.Credits,
                    Grade = e.Grade
                }).ToList()
            };
        }

        // Aggregate over a nested collection + OrderByDescending + Take
        public async Task<List<TopCollegeDTO>> GetTopCollegesAsync(int count)
        {
            return await _db.StudentsDetails
                .Select(c => new TopCollegeDTO
                {
                    Name = c.Name,
                    Dept = c.Dept,
                    StudentCount = c.Departments.Sum(d => d.Students.Count)
                })
                .OrderByDescending(x => x.StudentCount)
                .Take(count)
                .ToListAsync();
        }

        // Count + Average per course
        public async Task<List<CourseStatDTO>> GetCourseStatsAsync()
        {
            return await _db.Courses
                .Select(c => new CourseStatDTO
                {
                    Title = c.Title,
                    Department = c.Department!.Name,
                    EnrolledCount = c.Enrollments.Count,
                    AverageGrade = c.Enrollments.Average(e => e.Grade)
                })
                .OrderByDescending(x => x.EnrolledCount)
                .ToListAsync();
        }

        // Dynamic filtering + paging (Where, Skip, Take, CountAsync)
        public async Task<PagedResultDTO<StudentListItemDTO>> SearchStudentsAsync(
            string? search, int? departmentId, int page, int pageSize)
        {
            var query = _db.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var key = search.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(key)
                                      || s.Email.ToLower().Contains(key));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == departmentId.Value);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentListItemDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    Department = s.Department!.Name
                })
                .ToListAsync();

            return new PagedResultDTO<StudentListItemDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }
    }
}
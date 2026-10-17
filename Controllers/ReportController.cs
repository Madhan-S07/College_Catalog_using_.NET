using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Interface;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("department-summary")]
        public async Task<IActionResult> DepartmentSummary()
        {
            return Ok(await _service.GetDepartmentSummary());
        }

        [HttpGet("student/{id}")]
        public async Task<IActionResult> StudentDetail(int id)
        {
            var result = await _service.GetStudentDetail(id);
            if (result == null) return NotFound("Student not found");

            return Ok(result);
        }

        [HttpGet("top-colleges")]
        public async Task<IActionResult> TopColleges([FromQuery] int count = 3)
        {
            return Ok(await _service.GetTopColleges(count));
        }

        [HttpGet("course-stats")]
        public async Task<IActionResult> CourseStats()
        {
            return Ok(await _service.GetCourseStats());
        }

        [HttpGet("students")]
        public async Task<IActionResult> SearchStudents(
            [FromQuery] string? search,
            [FromQuery] int? departmentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.SearchStudents(search, departmentId, page, pageSize));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Models;
using CollegeManagementApi.Interface;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollegeController : ControllerBase
    {
        private readonly ICollegeService st_s;

        public CollegeController(ICollegeService _collegeService)
        {
            st_s = _collegeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await st_s.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await st_s.GetStudentById(id);
            if (student == null) return NotFound("Student not found");

            return Ok(student);
        }

        // POST: adds a new college, or updates NO_Students if it already exists
        [HttpPost]
        public async Task<IActionResult> Add(CollegeResponseDTO student)
        {
            var added = await st_s.AddStudent(student);
            if (!added) return BadRequest("Could not add student");

            return Ok("Student added");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await st_s.DeleteStudent(id);
            if (!deleted) return NotFound("Student not found");

            return Ok("deleted");
        }
    }
}
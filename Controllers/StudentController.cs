using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _service.GetById(id);
            if (student == null) return NotFound("Student not found");

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Add(StudentRequestDTO dto)
        {
            var (student, error) = await _service.Add(dto);
            if (error != null) return BadRequest(error);

            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StudentRequestDTO dto)
        {
            var (student, error) = await _service.Update(id, dto);
            if (error == "Student not found") return NotFound(error);
            if (error != null) return BadRequest(error);

            return Ok(student);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound("Student not found");

            return Ok("deleted");
        }
    }
}
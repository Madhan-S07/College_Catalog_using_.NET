using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService service)
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
            var course = await _service.GetById(id);
            if (course == null) return NotFound("Course not found");

            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CourseRequestDTO dto)
        {
            var (course, error) = await _service.Add(dto);
            if (error != null) return BadRequest(error);

            return Ok(course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CourseRequestDTO dto)
        {
            var (course, error) = await _service.Update(id, dto);
            if (error == "Course not found") return NotFound(error);
            if (error != null) return BadRequest(error);

            return Ok(course);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound("Course not found");

            return Ok("deleted");
        }
    }
}
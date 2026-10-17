using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
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
            var enrollment = await _service.GetById(id);
            if (enrollment == null) return NotFound("Enrollment not found");

            return Ok(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EnrollmentRequestDTO dto)
        {
            var (enrollment, error) = await _service.Add(dto);
            if (error != null) return BadRequest(error);

            return Ok(enrollment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EnrollmentRequestDTO dto)
        {
            var (enrollment, error) = await _service.Update(id, dto);
            if (error == "Enrollment not found") return NotFound(error);
            if (error != null) return BadRequest(error);

            return Ok(enrollment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound("Enrollment not found");

            return Ok("deleted");
        }
    }
}
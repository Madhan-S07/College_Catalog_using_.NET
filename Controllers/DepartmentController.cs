using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.Interface;
using CollegeManagementApi.Models;

namespace CollegeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
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
            var department = await _service.GetById(id);
            if (department == null) return NotFound("Department not found");

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentRequestDTO dto)
        {
            var (department, error) = await _service.Add(dto);
            if (error != null) return BadRequest(error);

            return Ok(department);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DepartmentRequestDTO dto)
        {
            var (department, error) = await _service.Update(id, dto);
            if (error == "Department not found") return NotFound(error);
            if (error != null) return BadRequest(error);

            return Ok(department);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound("Department not found");

            return Ok("deleted");
        }
    }
}
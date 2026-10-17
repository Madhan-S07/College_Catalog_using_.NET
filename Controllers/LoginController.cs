using Microsoft.AspNetCore.Mvc;
using CollegeManagementApi.DTO;
using CollegeManagementApi.Models;
using CollegeManagementApi.Services;
namespace CollegeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ILoginService loginService;
        public LoginController(ILoginService _loginService)
        {
            loginService = _loginService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestDTO tourDTO)
        {
            var res = await loginService.PerformLogin(tourDTO.user, tourDTO.password);
            if (res != null)
                return Ok(res);
            else
                return BadRequest("Login failed");
        }
        [HttpPost("CreateAccount")]
       
        public async Task<ActionResult<string>> CreateAccount([FromBody] LoginRequestDTO tourDTO)
        {
            var created = await loginService.CreateAccount(tourDTO.user, tourDTO.password);
            if (!created) return BadRequest("Username already exists");

            return Ok("Account created");
        }
            }
}
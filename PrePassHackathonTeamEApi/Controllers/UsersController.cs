using Microsoft.AspNetCore.Mvc;

namespace PrePassHackathonTeamEApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            // This is a sample response - you would typically get this from a database
            var users = new[]
            {
                new { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
            };

            return Ok(users);
        }
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportUserController : ControllerBase
    {
        private readonly ILogger<ReportUserController> _logger;
        private readonly IUserService _userService;

        public ReportUserController(ILogger<ReportUserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("sessionlogins")]
        public async Task<IActionResult> Get(int userId, int size = 50)
        {
            var data = await _userService.GetLoginsByUserIdAsync(userId, size);

            return Ok(new ResponseDTO<List<UserLoginDTO>>($"List of the last {size} login sessions", data));
        }
    }
}

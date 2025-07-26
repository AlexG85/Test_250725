using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest credential)
        {
            if (credential is null)
                return BadRequest(new ResponseDTO<string>("Invalid login request.") { Success = false });

            try
            {
                var response = await _userService.AuthenticateAsync(credential);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [AllowAnonymous]
        [HttpPost("Signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest data)
        {
            try
            {
                await _userService.AddAndUpdateUserAsync(data);

                return Ok(new ResponseDTO<string>("User created correctly"));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest data)
        {
            if (data is null)
                return BadRequest(new ResponseDTO<string>("Invalid client request.") { Success = false });

            try
            {
                var response = await _userService.RefreshAuthenticationAsync(data.Token, data.Refresh);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("GetAuthenticated")]
        public IActionResult GetName()
        {
            return Ok(new ResponseDTO<string>($"User Authenticated: {User.Identity?.Name}"));
        }
    }
}

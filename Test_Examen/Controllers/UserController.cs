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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new ResponseDTO<List<UserDTO>>("List of users", users));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(new ResponseDTO<AppUser>("Role found", user));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserRequest userDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(userDto, User.Identity.Name);
                return Ok(new ResponseDTO<string>("User updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new ResponseDTO<string>("User deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }
    }
}

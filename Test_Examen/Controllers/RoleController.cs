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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(new ResponseDTO<List<RoleDTO>>("List of roles", roles));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                return Ok(new ResponseDTO<AppRole>("Role found", role));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] string roleName)
        {
            try
            {
                var result = await _roleService.AddRoleAsync(roleName);                
                return Ok(new ResponseDTO<string>("Role created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromBody] RoleUpdateRequest roleDto)
        {
            try
            {
                var result = await _roleService.UpdateRoleAsync(roleDto);
                return Ok(new ResponseDTO<string>("Role updated successfully"));
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
                var result = await _roleService.DeleteRole(id);
                return Ok(new ResponseDTO<string>("Role deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPost("{roleId}/{userId}")]
        public async Task<IActionResult> UpdateRoleUser(int roleId, int userId)
        {
            try
            {
                var result = await _roleService.UpdateUserRoleAsync(userId, roleId);
                return Ok(new ResponseDTO<string>("Role updated for user successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }
    }
}

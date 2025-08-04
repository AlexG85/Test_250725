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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _employeeService.GetAllAsync();
            return Ok(new ResponseDTO<List<EmployeeDTO>>("List of employees", data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _employeeService.GetByIdAsync(id);
                return Ok(new ResponseDTO<Employee>("Employee found", data));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] EmployeeDTO employee)
        {
            try
            {
                var result = await _employeeService.AddEmployeeAsync(employee);                
                return Ok(new ResponseDTO<string>("Employee created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromBody] EmployeeDTO employeeDTO)
        {
            try
            {
                var result = await _employeeService.UpdateEmployeeAsync(employeeDTO);
                return Ok(new ResponseDTO<string>("Employee updated successfully"));
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
                var result = await _employeeService.DeleteEmployee(id);
                return Ok(new ResponseDTO<string>("Employee deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO<string>(ex.Message) { Success = false });
            }
        }
    }
}

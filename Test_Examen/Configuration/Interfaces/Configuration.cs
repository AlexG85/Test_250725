using System.Security.Claims;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Configuration.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> AuthenticateAsync(AuthenticationRequest model);

        Task<List<UserDTO>> GetAllAsync(bool status = true);

        Task<AppUser> GetByIdAsync(int id);

        Task<bool> AddUserAsync(SignInRequest request);

        Task<bool> UpdateUserAsync(UserRequest user, string identity);

        Task<bool> DeleteUserAsync(int userId);

        Task<AuthenticateResponse?> RefreshAuthenticationAsync(string token, string refreshToken);

        Task<List<UserLoginDTO>> GetLoginsByUserIdAsync(int id, int size = 50);
    }

    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllAsync(bool status = true);

        Task<AppRole> GetByIdAsync(int id);

        Task<bool> AddRoleAsync(string description);

        Task<bool> UpdateRoleAsync(RoleUpdateRequest role);

        Task<bool> UpdateUserRoleAsync(int userId, int roleId);

        Task<bool> DeleteRole(int roleId);
    }

    public interface IEmployeeService
    {
        Task<List<EmployeeDTO>> GetAllAsync(bool status = true);

        Task<Employee> GetByIdAsync(int id);

        Task<bool> AddEmployeeAsync(EmployeeDTO employee);

        Task<bool> UpdateEmployeeAsync(EmployeeDTO employee);

        Task<bool> DeleteEmployee(int employeeId);
    }
}

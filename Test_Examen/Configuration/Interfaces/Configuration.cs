using System.Security.Claims;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Configuration.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> AuthenticateAsync(AuthenticationRequest model);

        Task<List<AppUser>> GetAllAsync(bool status = true);

        Task<AppUser> GetByIdAsync(int id);

        Task<bool> AddAndUpdateUserAsync(SignInRequest request);

        Task<AuthenticateResponse?> RefreshAuthenticationAsync(string token, string refreshToken);

        Task<List<UserLoginDTO>> GetLoginsByUserIdAsync(int id, int size = 50);
    }
}

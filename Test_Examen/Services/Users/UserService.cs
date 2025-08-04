using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Test_Examen.Configuration.Database;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Helpers;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Test_Examen.Services.Users
{
    public class UserService : IUserService
    {
        private readonly JwtAuthConfig _jwtSettings;
        private readonly SQLDBContext db;
        private readonly IMemoryCache _memoryCache;
        private readonly JwtSecurityTokenHelper _jwtTokenHelper;

        public UserService(IOptions<JwtAuthConfig> jwtSettings, SQLDBContext _db, IMemoryCache memoryCache, JwtSecurityTokenHelper jwtTokenHelper)
        {
            _jwtSettings = jwtSettings.Value;
            db = _db;
            _memoryCache = memoryCache;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// Authenticate users
        /// </summary>
        /// <param name="model">request for password and email</param>
        /// <returns>Authentication tokens</returns>
        /// <exception cref="Exception">Reason for not authenticate</exception>
        public async Task<AuthenticateResponse?> AuthenticateAsync(AuthenticationRequest model)
        {
            var user = await db.Users.SingleOrDefaultAsync(x => x.UserName == model.UserName);

            if (user == null) 
                throw new Exception("User does not exists");

            if (user.LockoutEnabled) {
                if (DateTimeOffset.UtcNow > user.LockoutEnd)
                {
                    user.LockoutEnd = null;
                    user.LockoutEnabled = false;
                    user.AccessFailedCount = 0;
                }
                else
                    throw new Exception("User is locked out");
            }

            if (user.PasswordHash == model.Password)
            {
                var (newToken, newRefreshToken) = await GenerateJwtAndRefreshTokenAsync(user);

                await AddLogin(user.Id, "JWT", newRefreshToken);

                return new AuthenticateResponse(user, newToken, newRefreshToken);
            }
            else
            {
                user.AccessFailedCount += 1;
                user.LockoutEnabled = user.AccessFailedCount >= _jwtSettings.LoginAttemps;
                user.LockoutEnd = user.LockoutEnabled ? DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.LockoutTime) : null;

                await db.SaveChangesAsync();

                throw new Exception($"Invalid credentials. Please try again. {_jwtSettings.LoginAttemps - user.AccessFailedCount} remaining retries");
            }
        }

        /// <summary>
        /// Refresh the tokens
        /// </summary>
        /// <param name="token">current token</param>
        /// <param name="refreshToken">refresh token id</param>
        /// <returns>New Authentication tokens</returns>
        /// <exception cref="Exception">Reason for failing</exception>
        public async Task<AuthenticateResponse?> RefreshAuthenticationAsync(string token, string refreshToken)
        {
            // Validates the digital signature of the provided access token
            var validatedToken = await _jwtTokenHelper.GetPrincipalFromToken(token);
            if (validatedToken is null)
                throw new Exception("Invalid Token");

            var jti = validatedToken.Claims
                .SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti))
                throw new Exception("Invalid Token: JTI claim not found");

            var storedRefreshToken = await db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken is null)
                throw new Exception("This refresh token does not exist");

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                throw new Exception("This refresh token has expired");

            if (storedRefreshToken.Invalidated || storedRefreshToken.JwtId != jti)
                throw new Exception("This refresh token has been invalidated");

            var userId = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                throw new Exception("User not found in the token claims");

            // Search for a user in the database
            int _userId = int.Parse(userId);
            var user = await GetByIdAsync(_userId);
            if (user is null)
                throw new Exception("User not found");

            await AddToBlacklist(jti, _userId);

            // Creates a brand new pair of tokens (access token and refresh token)
            var (newToken, newRefreshToken) = await GenerateJwtAndRefreshTokenAsync(user, token);

            return new AuthenticateResponse(user, newToken, newRefreshToken);
        }

        /// <summary>
        /// Get all Active or not Users
        /// </summary>
        /// <param name="status">True if only active users</param>
        /// <returns>List for all required users</returns>
        public async Task<List<UserDTO>> GetAllAsync(bool status = true)
        {
            var data = await db.Users.Where(x => x.IsActive == status || !status)
                .AsNoTracking()
                .Select(x => new UserDTO
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    UserName = x.UserName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ModifiedAt = x.ModifiedAt,
                    ModifiedBy = x.ModifiedBy
                })
                .ToListAsync();

            return data;
        }

        /// <summary>
        /// Get an User entity
        /// </summary>
        /// <param name="id">UserId to get</param>
        /// <returns>Users entity info</returns>
        /// <exception cref="Exception">Reason for failing</exception>
        public async Task<AppUser> GetByIdAsync(int id)
        {
            var data = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (data == null)
                throw new Exception("User not found");

            return data;
        }

        /// <summary>
        /// Creates a new User for the application
        /// </summary>
        /// <param name="request">request with the new data</param>
        /// <returns>Text if the user was created correctly</returns>
        /// <exception cref="Exception">User already exists or DB error</exception>
        public async Task<bool> AddUserAsync(SignInRequest request)
        {
            var obj = await db.Users.SingleOrDefaultAsync(c => c.UserName == request.EMail);
            if (obj != null)
                throw new Exception("User already exists with this email address.");

            var _roleId = (await db.Roles.FirstAsync(r => r.Description == "DefaultRole")).RoleId;

            await db.Users.AddAsync(new AppUser()
            {
                UserName = request.EMail,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = request.Password,
                Email = request.EMail,
                IsActive = true,
                RoleId = _roleId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            });

            if (await db.SaveChangesAsync() == 0)
                throw new Exception("Failed to create user. Please try again later.");

            return true;
        }

        public async Task<bool> UpdateUserAsync(UserRequest user, string identity)
        {
            bool hasUser = await db.Users.AnyAsync(c => c.UserName == user.UserName);
            if (!hasUser)
                throw new Exception("User does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Users.Where(x => x.UserName == user.UserName)
                    .ExecuteUpdateAsync(u => 
                        u.SetProperty(x => x.FirstName, user.FirstName)
                         .SetProperty(x => x.LastName, user.LastName)
                         .SetProperty(x => x.Email, user.Email)
                         .SetProperty(x => x.ModifiedBy, identity)
                         .SetProperty(x => x.ModifiedAt, DateTime.UtcNow)
                    );

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            bool hasUser = await db.Users.AnyAsync(c => c.Id == userId);
            if (!hasUser)
                throw new Exception("User does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Users.Where(x => x.Id == userId)
                    .ExecuteDeleteAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }

        /// <summary>
        /// Get a customized size list of and specific user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="size">Size of the list, By default = 50</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<UserLoginDTO>> GetLoginsByUserIdAsync(int id, int size = 50)
        {
            var user = await db.Users
                            .Include(x => x.Logins.OrderByDescending(l => l.CreationDate))
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                throw new Exception("User not found");

            return user.Logins.Select(x => new UserLoginDTO {
                    LoginProvider = x.LoginProvider,
                    ProviderKey = x.ProviderKey,
                    CreationDate = x.CreationDate
                })
                .Take(size).ToList();
        }

        #region "Procedures for special Functions"

        /// <summary>
        /// Add a new record
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="provider">Specify which Authentication method used</param>
        /// <param name="providerId">Specify an id for the Provider</param>
        /// <returns></returns>
        private async Task<bool> AddLogin(int userId, string provider, string providerId)
        {
            var loginUser = new AppUserLogin
            {
                UserId = userId,
                LoginProvider = provider,
                ProviderKey = providerId
            };

            try
            {
                await db.AddAsync(loginUser);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a token id to Blacklist and memory
        /// </summary>
        /// <param name="jti">Token ide ntificator</param>
        /// <param name="userId">User identificador</param>
        /// <returns></returns>
        private async Task<bool> AddToBlacklist(string jti, int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = jti,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow,
                Invalidated = true
            };

            try
            {
                _memoryCache.Set(jti, userId);

                await db.AddAsync(refreshToken);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region "Tools for Token Generations"

        /// <summary>
        /// Get the Token and Refresh token
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="existingRefreshToken">Existing refresh token</param>
        /// <returns>New Authentication tokens</returns>
        private async Task<(string token, string refreshToken)> GenerateJwtAndRefreshTokenAsync(AppUser user, string? existingRefreshToken = null)
        {
            var token = await GenerateJwtToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(token, user, existingRefreshToken);

            return (token, refreshToken);
        }

        /// <summary>
        /// Generate the new token
        /// </summary>
        /// <param name="user">User Entity</param>
        /// <returns>New generated token</returns>
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SigningKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    Subject = new ClaimsIdentity([
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        ]),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generate the new Refresh token
        /// </summary>
        /// <param name="token">Current token</param>
        /// <param name="user">User entity</param>
        /// <param name="existingRefreshToken">Current refresh token</param>
        /// <returns></returns>
        private async Task<string> GenerateRefreshTokenAsync(string token, AppUser user, string? existingRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var jti = jwtToken.Id;

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = jti,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAtUtc = DateTime.UtcNow,
            };

            if (!string.IsNullOrEmpty(existingRefreshToken))
            {
                var existingToken = await db.RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == existingRefreshToken);

                if (existingToken != null)
                    db.Set<RefreshToken>().Remove(existingToken);
            }

            await db.AddAsync(refreshToken);
            await db.SaveChangesAsync();

            return refreshToken.Token;
        }


        #endregion

    }
}

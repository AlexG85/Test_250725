using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Configuration.Helpers
{
    public class JwtSecurityTokenHelper
    {
        private readonly JwtAuthConfig _jwtSettings;

        public JwtSecurityTokenHelper(IOptions<JwtAuthConfig> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Verify if the token is valid
        /// </summary>
        /// <param name="token">Current token to validate</param>
        /// <returns>Claim Principal</returns>
        /// <exception cref="SecurityTokenException">Message for invalid token</exception>
        public async Task<ClaimsPrincipal> GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey))
            };

            return await Task.Run(() => {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            });
        }
    }
}

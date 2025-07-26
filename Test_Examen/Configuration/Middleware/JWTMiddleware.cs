using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test_Examen.Configuration.Database;
using Test_Examen.Configuration.Helpers;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Configuration.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtAuthConfig _jwtSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly JwtSecurityTokenHelper _jwtTokenHelper;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtAuthConfig> jwtSettings, IMemoryCache memoryCache, JwtSecurityTokenHelper jwtTokenHelper)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
            _memoryCache = memoryCache;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/login") || context.Request.Path.StartsWithSegments("/refresh"))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claimsPrincipal = await _jwtTokenHelper.GetPrincipalFromToken(token);

                    var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    context.Items["UserId"] = userId;

                    var jwtId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                    if (jwtId == null) throw new Exception("404 - Authorization failed - Invalid Scope");

                    var revocationType = _memoryCache.Get(jwtId);
                    if (revocationType != null) throw new Exception("404 - Authorization failed - Blacklisted");

                    var identity = new ClaimsIdentity(claimsPrincipal.Claims, "basic");
                    context.User = new ClaimsPrincipal(identity);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next(context);
        }

    }
}

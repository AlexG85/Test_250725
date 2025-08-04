using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using System.Text;
using Test_Examen.Configuration.Database;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Helpers;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Middleware;
using Test_Examen.Configuration.Models;
using Test_Examen.Configuration.Services;
using Test_Examen.Services.Employees;
using Test_Examen.Services.Roles;
using Test_Examen.Services.Users;


var builder = WebApplication.CreateBuilder(args);

// Custom Services
#region "Authentication"

var optJwt = new JwtAuthConfig();
builder.Configuration.GetSection("Authentication:Jwt").Bind(optJwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        // Configure the Authority to the expected value for the authentication provider. This ensures the token is appropriately validated.
        opt.Authority = optJwt.Issuer; // TODO: Update URL
        opt.RequireHttpsMetadata = false;
        
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = optJwt.Issuer,
            ValidAudience = optJwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(optJwt.SigningKey))
        };
    });

#endregion

#region Services
builder.Services.Configure<JwtAuthConfig>(builder.Configuration.GetSection("Authentication:Jwt"));

builder.Services.AddMemoryCache();

// Configuring SQL DB connection
var connDBString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SQLDBContext>(options => options.UseSqlServer(connDBString));

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build());

builder.Services.AddHostedService<InvalidTokenService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddSingleton<JwtSecurityTokenHelper>();

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbcontext = scope.ServiceProvider.GetRequiredService<SQLDBContext>();
    if (dbcontext.Database.IsRelational())
        dbcontext.Database.Migrate(); // Apply any pending migrations for the context to the database
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

#region "Custom Middleware"

app.UseMiddleware<JwtMiddleware>();

#endregion

app.MapControllers();

app.Run();

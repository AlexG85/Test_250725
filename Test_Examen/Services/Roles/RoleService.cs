using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Test_Examen.Configuration.Database;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly SQLDBContext db;

        public RoleService(SQLDBContext dbContext)
        {
            db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> AddRoleAsync(string description)
        {
            bool hasRole = await db.Roles.AnyAsync(c => c.Description.Contains(description));
            if (hasRole)
                throw new Exception("Role already exists with this description.");

            await db.Roles.AddAsync(new AppRole() {
                Description = description,
                IsActive = true
            });

            if (await db.SaveChangesAsync() == 0)
                throw new Exception("Failed to create role. Please try again later.");

            return true;
        }

        public async Task<List<RoleDTO>> GetAllAsync(bool status = true)
        {
            var data = await db.Roles.Where(x => x.IsActive == status || !status)
               .AsNoTracking()
               .Select(x => new RoleDTO
               {
                   RoleId = x.RoleId,
                   Description = x.Description,
                   IsActive = x.IsActive
               })
               .ToListAsync();

            return data;
        }

        public async Task<AppRole> GetByIdAsync(int id)
        {
            var data = await db.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.RoleId == id);

            if (data == null)
                throw new Exception("Role not found");

            return data;
        }

        public async Task<bool> UpdateRoleAsync(RoleUpdateRequest role)
        {
            bool hasRole = await db.Roles.AnyAsync(c => c.RoleId == role.RoleID);
            if (!hasRole)
                throw new Exception("Role does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Roles.Where(x => x.RoleId == role.RoleID)
                    .ExecuteUpdateAsync(r => 
                        r.SetProperty(r => r.Description, role.Description)
                         .SetProperty(r => r.IsActive, role.IsActive));

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }

        public async Task<bool> DeleteRole(int roleId)
        {
            bool hasRole = await db.Roles.AnyAsync(c => c.RoleId == roleId);
            if (!hasRole)
                throw new Exception("Role does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Roles.Where(c => c.RoleId == roleId).ExecuteDeleteAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            bool hasRole = await db.Roles.AnyAsync(c => c.RoleId == roleId);
            if (!hasRole)
                throw new Exception("Role does not exists.");

            bool hasUser = await db.Users.AnyAsync(c => c.Id == userId);
            if (!hasUser)
                throw new Exception("User does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Users.Where(x => x.Id == userId)
                    .ExecuteUpdateAsync(u => u.SetProperty(u => u.RoleId, roleId));

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }

    }
}

using Microsoft.EntityFrameworkCore;
using System.Data;
using Test_Examen.Configuration.Database;
using Test_Examen.Configuration.Entities;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;

namespace Test_Examen.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly SQLDBContext db;

        public EmployeeService(SQLDBContext dbContext)
        {
            db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> AddEmployeeAsync(EmployeeDTO employee)
        {
            bool hasEmployee = await db.Employees.AnyAsync(c => c.Email.Contains(employee.Email));
            if (hasEmployee)
                throw new Exception("Employee already exists with this email.");

            await db.Employees.AddAsync(new Employee()
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Position = employee.Position,
                Department = employee.Department,
                IsActive = true
            });

            if (await db.SaveChangesAsync() == 0)
                throw new Exception("Failed to create employee. Please try again later.");

            return true;
        }

        public async Task<bool> DeleteEmployee(int employeeId)
        {
            bool hasEmployee = await db.Employees.AnyAsync(c => c.Id == employeeId);
            if (!hasEmployee)
                throw new Exception("Employee does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Employees.Where(c => c.Id == employeeId).ExecuteDeleteAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return true;
        }

        public async Task<List<EmployeeDTO>> GetAllAsync(bool status = true)
        {
            var data = await db.Employees.Where(x => x.IsActive == status || !status)
               .AsNoTracking()
               .Select(x => new EmployeeDTO()
               {
                   Id = x.Id,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Email = x.Email,
                   Position = x.Position,
                   Department = x.Department
               })
               .ToListAsync();

            return data;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var data = await db.Employees.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (data == null)
                throw new Exception("Employee not found");

            return data;
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeDTO employee)
        {
            bool hasEmployee = await db.Employees.AnyAsync(c => c.Id == employee.Id);
            if (!hasEmployee)
                throw new Exception("Employee does not exists.");

            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await db.Employees.Where(x => x.Id == employee.Id)
                    .ExecuteUpdateAsync( r =>
                        r.SetProperty(r => r.FirstName, employee.FirstName)
                         .SetProperty(r => r.LastName, employee.LastName)
                         .SetProperty(r => r.Email, employee.Email)
                         .SetProperty(r => r.Position, employee.Position)
                         .SetProperty(r => r.Department, employee.Department)
                         .SetProperty(r => r.IsActive, true) // Assuming we want to keep the employee active
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
    }
}

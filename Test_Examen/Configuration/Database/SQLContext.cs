using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test_Examen.Configuration.Entities;


namespace Test_Examen.Configuration.Database
{
    public class SQLDBContext : DbContext
    {
        public SQLDBContext(DbContextOptions<SQLDBContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList()
                .ForEach(entityEntry => {
                    if (entityEntry.Metadata.FindProperty("ModifiedAt") != null && entityEntry.State == EntityState.Modified)
                        entityEntry.Property("ModifiedAt").CurrentValue = DateTime.UtcNow;

                    if (entityEntry.Metadata.FindProperty("CreatedAt") != null && entityEntry.State == EntityState.Added)
                        entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                });

            return base.SaveChanges();
        }

        #region "DBSets"
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserLogin> UserLogins { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Employee> Employees { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b =>
            {
                b.HasKey(e => e.Id).HasName("PK_tUser_Id");

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens).WithOne().HasForeignKey(ut => ut.UserId).HasConstraintName("FK_Users_Tokens").IsRequired();
                // Each User can have many UserLogins
                b.HasMany(e => e.Logins).WithOne().HasForeignKey(ul => ul.UserId).HasConstraintName("FK_Users_Logins").IsRequired();

                b.HasIndex(p => p.UserName).HasDatabaseName("IX_Users_Username");
                b.HasIndex(p => new { p.LastName, p.FirstName }).HasDatabaseName("IX_Users_FullName");
                
                b.ToTable("app_tUsers");
            });

            builder.Entity<AppUserLogin>(b => {
                b.HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey }).HasName("PK_UserLogins_UserProviderKey");

                b.HasOne(l => l.User).WithMany(i => i.Logins).HasForeignKey(o => o.UserId);
                
                b.ToTable("app_rUserLogins");
            });

            builder.Entity<RefreshToken>(b => {
                b.HasKey(t => new { t.UserId, t.Token }).HasName("PK_RefreshToken_UserIdToken");

                b.HasOne(l => l.User).WithMany(i => i.Tokens).HasForeignKey(o => o.UserId);

                b.ToTable("app_tRefreshTokens");
            });

            builder.Entity<AppRole>(b =>
            {
                b.HasKey(r => r.RoleId).HasName("PKRole_RoleId");

                b.HasData(new AppRole() { RoleId = 1, Description = "DefaultRole", IsActive = true });

                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.RoleUsers).WithOne(e => e.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany(e => e.RolePermissions).WithOne(e => e.Role).HasForeignKey(rp => rp.RoleId).IsRequired();

                b.ToTable("tRoles");
            });

            builder.Entity<Module>(b =>
            {
                b.HasKey(m => m.ModuleId).HasName("PKModule_ModuleId");

                b.HasMany(e => e.Permissions).WithOne(e => e.Module).HasForeignKey(mp => mp.ModuleType).IsRequired();
                b.HasData(new Module() { ModuleId = ModuleType.General, Name = "General", Description = "General Module" });

                b.ToTable("tModules");

            });

            builder.Entity<Permission>(b =>
            {
                b.HasKey(p => p.PermissionId).HasName("PKPermission_PermissionId");

                b.HasIndex(e => new { e.ModuleType, e.PermissionType }).HasDatabaseName("IX_Permission_ModulePermission").IsUnique();
                b.HasMany(e => e.RolePermissions).WithOne(e => e.Permission).HasForeignKey(rp => rp.PermissionId).IsRequired();

                b.ToTable("tPermissions");
            });

            builder.Entity<RolePermission>(b =>
            {
                b.HasKey(rp => new { rp.RoleId, rp.PermissionId }).HasName("PKRolePermission_RolePermission");

                b.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId).IsRequired();
                b.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId).IsRequired();

                b.ToTable("rRolePermissions");
            });

            builder.Entity<Employee>(b =>
            {
                b.HasKey(e => e.Id).HasName("PKEmployee_EmployeeId");

                b.HasIndex(e => e.LastName).HasDatabaseName("IX_Employees_LastName");
                b.HasIndex(e => e.FirstName).HasDatabaseName("IX_Employees_FirstName");

                b.ToTable("tEmployees");
            });
        }
    }
}

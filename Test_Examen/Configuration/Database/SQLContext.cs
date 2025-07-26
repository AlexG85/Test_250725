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

        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserLogin> UserLogins { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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


        }
    }
}

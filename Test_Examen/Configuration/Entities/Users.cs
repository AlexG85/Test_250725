using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security;

namespace Test_Examen.Configuration.Entities
{
    public class AppUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(250)]
        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpireToken { get; set; } = DateTime.UtcNow.AddDays(7);

        [Required]
        public bool IsActive { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(30)]
        public string CreatedBy { get; set; } = "System";

        [Required]
        public DateTime ModifiedAt { get; set; }

        [Required]
        [StringLength(30)]
        public string ModifiedBy { get; set; } = "System";

        public string PasswordHash { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        [Required]
        public bool LockoutEnabled { get; set; } = false;

        [Required]
        public int AccessFailedCount { get; set; } = 0;

        public virtual ICollection<AppUserLogin> Logins { get; set; } = null!;
        public virtual ICollection<RefreshToken> Tokens { get; set; } = null!;
    }

    public class AppUserLogin
    {
        public int UserId { get; set; }

        [StringLength(450)]
        public string LoginProvider { get; set; }

        [StringLength(450)]
        public string ProviderKey { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public virtual AppUser User { get; set; } // Relación con app_tUsers
    }

    public class RefreshToken
    {
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string JwtId { get; set; } = null!;

        [Required]
        public DateTime CreatedAtUtc { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool Invalidated { get; set; }

        public virtual AppUser User { get; set; } = null!;
    }
}

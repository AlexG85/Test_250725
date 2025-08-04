using System.ComponentModel.DataAnnotations;

namespace Test_Examen.Configuration.Models
{
    public class UserLoginDTO
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }

    public class UserDTO
    {
        public int Id { get; set; }

        [Required, StringLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Email { get; set; } = string.Empty;

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
    }

    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Examen.Configuration.Entities
{
    public class AppRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<AppUser> RoleUsers { get; set; } = null!;
        public virtual ICollection<RolePermission> RolePermissions { get; } = [];
    }
}

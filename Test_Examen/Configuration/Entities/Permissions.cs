using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Test_Examen.Configuration.Entities
{
    public class Module
    {
        [Column(TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ModuleType ModuleId { get; set; }

        [Required, Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        [Required, Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;

        public virtual ICollection<Permission> Permissions { get; } = [];
    }

    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PermissionId { get; set; }

        [Column(TypeName = "int")]
        public ModuleType ModuleType { get; set; }

        [Column(TypeName = "int")]
        public PermissionType PermissionType { get; set; }

        [Required, Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        [Required, Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;

        public virtual Module Module { get; set; } = null!;

        public virtual ICollection<RolePermission> RolePermissions { get; } = [];
    }

    public class RolePermission
    {
        [Column(Order = 1)]
        public int RoleId { get; set; }

        [Column(Order = 2, TypeName = "int")]
        public int PermissionId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;

        public virtual AppRole Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;

    }
}

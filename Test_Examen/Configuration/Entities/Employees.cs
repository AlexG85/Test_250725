using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Test_Examen.Configuration.Entities
{
    public class Employee : EmployeeDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }

    public class EmployeeDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string Position { get; set; } = string.Empty;

        [StringLength(50)]
        public string Department { get; set; } = string.Empty;
    }
}

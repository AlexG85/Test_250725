using System.ComponentModel.DataAnnotations;

namespace Test_Examen.Configuration.Models
{
    public class UserLoginDTO
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}

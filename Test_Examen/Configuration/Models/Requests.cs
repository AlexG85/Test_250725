using System.ComponentModel.DataAnnotations;

namespace Test_Examen.Configuration.Models
{
    public class AuthenticationRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignInRequest()
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest(string token, string refresh)
    {
        public string Token { get; set; } = token;
        public string Refresh { get; set; } = refresh;
    }

    public class UserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class RoleUpdateRequest
    {
        public int RoleID { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }


}

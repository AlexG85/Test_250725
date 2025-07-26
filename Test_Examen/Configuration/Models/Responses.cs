using Test_Examen.Configuration.Entities;

namespace Test_Examen.Configuration.Models
{
    public class ResponseDTO<T> where T : class
    {
        public ResponseDTO(string message, T? data = null)
        {
            Message = message;
            Data = data;
        }

        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } = null;
    }

    public class AuthenticateResponse(AppUser user, string token, string refresh)
    {
        public int Id { get; set; } = user.Id;
        public string FirstName { get; set; } = user.FirstName;
        public string LastName { get; set; } = user.LastName;
        public string Username { get; set; } = user.UserName;
        public string Token { get; set; } = token;
        public string Refresh { get; set; } = refresh;
    }
}

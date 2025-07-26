namespace Test_Examen.Configuration.Models
{
    public class JwtAuthConfig
    {
        public int ExpireMinutes { get; set; } = 1;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
        public int LoginAttemps { get; set; } = 5;
        public int LockoutTime { get; set; } = 30;
    }
}

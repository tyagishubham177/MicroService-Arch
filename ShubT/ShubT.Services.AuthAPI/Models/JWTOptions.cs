namespace ShubT.Services.AuthAPI.Models
{
    public class JWTOptions
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
        public int ExpiryInDays { get; set; }
    }
}

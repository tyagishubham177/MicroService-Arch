namespace ShubT.Services.AuthAPI.DTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}

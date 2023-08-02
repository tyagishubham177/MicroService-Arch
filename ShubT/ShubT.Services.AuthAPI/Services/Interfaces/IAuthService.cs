using ShubT.Services.AuthAPI.DTOs;

namespace ShubT.Services.AuthAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDTO> LoginUserAsync(LoginRequestDTO model);
        Task<bool> AssignRoleAsync(string email, string roleName);
    }
}

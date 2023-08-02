using ShubT.Web.Models;
using ShubT.Web.Models.Auth;

namespace ShubT.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<ResponseDTO> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO);
    }
}

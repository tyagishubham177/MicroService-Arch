using ShubT.Web.Models;
using ShubT.Web.Models.Auth;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;

namespace ShubT.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDTO,
                Url = MiscUtils.AuthAPIBase + "/api/auth/assignRole"
            });
        }

        public async Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDTO,
                Url = MiscUtils.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDTO,
                Url = MiscUtils.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}

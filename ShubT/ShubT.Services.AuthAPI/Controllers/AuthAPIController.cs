using Microsoft.AspNetCore.Mvc;
using ShubT.Services.AuthAPI.DTOs;
using ShubT.Services.AuthAPI.Services.Interfaces;

namespace ShubT.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _responseDTO;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDTO = new ResponseDTO();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var errorMessage = await _authService.RegisterUserAsync(model);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = errorMessage;
                return Ok(_responseDTO);
            }

            return Ok(_responseDTO);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _authService.LoginUserAsync(model);
            if (loginResponse.User == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = "Invalid username or password";
                return Ok(_responseDTO);
            }

            _responseDTO.Result = loginResponse;
            return Ok(_responseDTO);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            var response = await _authService.AssignRoleAsync(model.Email, model.Role.ToUpper());
            if (!response)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = "Unable to assign role";
                return BadRequest(_responseDTO);
            }
            return Ok(_responseDTO);
        }
    }
}

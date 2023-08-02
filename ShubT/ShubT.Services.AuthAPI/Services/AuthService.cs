using Microsoft.AspNetCore.Identity;
using ShubT.Services.AuthAPI.Data;
using ShubT.Services.AuthAPI.DTOs;
using ShubT.Services.AuthAPI.Models;
using ShubT.Services.AuthAPI.Services.Interfaces;

namespace ShubT.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJWTTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext dbContext, UserManager<AppUser> userManager
            , RoleManager<IdentityRole> roleManager, IJWTTokenGenerator jwtTokenGenerator)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRoleAsync(string email, string roleName)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> LoginUserAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.Username.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || !isValid)
            {
                return new LoginResponseDTO
                {
                    User = null,
                    Token = string.Empty
                };
            };

            var token = _jwtTokenGenerator.GenerateToken(user);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO
            {
                User = new UserDTO
                {
                    ID = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                },
                Token = token
            };
            return loginResponseDTO;
        }


        public async Task<string> RegisterUserAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            AppUser user = new AppUser()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _dbContext.AppUsers.First(u => u.UserName == registrationRequestDTO.Email);

                    var userDTO = new UserDTO
                    {
                        ID = userToReturn.Id,
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber,
                    };

                    return string.Empty;
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                return "Some MAJOR issue";
            }
        }
    }
}

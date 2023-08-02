﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShubT.Web.Models;
using ShubT.Web.Models.Auth;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShubT.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;

        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO requestDTO)
        {
            ResponseDTO response = await _authService.LoginAsync(requestDTO);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDTO responseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>(response.Result.ToString());
                await SignInUser(responseDTO);
                _tokenProvider.SetToken(responseDTO.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.DisplayMessage;
                //ModelState.AddModelError("CustomError", response.DisplayMessage);
                return View(requestDTO);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem {Text = MiscUtils.RoleAdmin, Value = MiscUtils.RoleAdmin},
                new SelectListItem {Text = MiscUtils.RoleCustomer, Value = MiscUtils.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO requestDTO)
        {
            ResponseDTO result = await _authService.RegisterAsync(requestDTO);
            ResponseDTO assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(requestDTO.Role))
                {
                    requestDTO.Role = MiscUtils.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(requestDTO);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.DisplayMessage;
            }
            var roleList = new List<SelectListItem>
            {
                new SelectListItem {Text = MiscUtils.RoleAdmin, Value = MiscUtils.RoleAdmin},
                new SelectListItem {Text = MiscUtils.RoleCustomer, Value = MiscUtils.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View(requestDTO);
        }

        //[HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDTO responseDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(responseDTO.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}

using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data.DTOs;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    public class AuthorizationController : BaseController
    {
        private readonly IUserService _userService;

        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("/User/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("/User/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("api/register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var registeredUser = await _userService.RegisterAsync(registrationDto);
                TempData["SuccessMessage"] = "Registration done successfully!";
                return Ok(new { Message = "Registration successful", User = registeredUser });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong.";
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserPhone", request.PhoneNumber);

                var user = await _userService.LoginAsync(request.PhoneNumber, request.Password);
                TempData["SuccessMessage"] = "Login successfully!";
                return Ok(new { Message = "Login successful", User = user });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong.";
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("api/logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return Ok(new { Message = "Logged out successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}

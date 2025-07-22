using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data.DTOs;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    public class AdminAuthController : BaseController
    {
        private readonly IUserService _userService;

        public AdminAuthController(IUserService userService)
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            _userService = userService;
        }
        [HttpGet("/Admin/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("/Admin/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/admin/api/register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var registeredUser = await _userService.RegisterAsync(registrationDto,true);
                return Ok(new { Message = "Registration successful", User = registeredUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("/admin/api/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserPhone", request.PhoneNumber);

                var user = await _userService.LoginAsync(request.PhoneNumber, request.Password);
                return Ok(new { Message = "Login successful", User = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("/admin/api/logout")]
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

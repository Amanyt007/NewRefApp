using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data;
using System.Linq;

namespace NewRefApp.Middlewares
{
    public class UserFilter : IAuthorizationFilter
    {
        private readonly ApplicationDbContext _context;

        public UserFilter(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            var rareentUser = request.Cookies["rareentUser"];
            var findUserCookie = request.Cookies["findUser"]; // contains "true"/"false"

            bool isAdmin = false;

            // If findUser cookie not set, fetch from DB once and set cookie
            if (string.IsNullOrEmpty(findUserCookie))
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == rareentUser);
                if (user != null)
                {
                    isAdmin = user.IsAdmin;

                    // Set cookie for future requests
                    response.Cookies.Append("findUser", isAdmin.ToString().ToLower(), new CookieOptions
                    {
                        Path = "/",
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(1) // valid for 1 hour
                    });
                }
            }
            else
            {
                // Parse the cookie value
                bool.TryParse(findUserCookie, out isAdmin);
            }

            // If not logged in or is actually an admin, redirect to login
            if (string.IsNullOrEmpty(rareentUser) || isAdmin)
            {
                context.Result = new RedirectToActionResult("Login", "Authorization", null);
                return;
            }

            // Debug log
            Console.WriteLine($"[UserFilter] Authenticated user: {rareentUser}, isAdmin: {isAdmin}");
        }
    }
}

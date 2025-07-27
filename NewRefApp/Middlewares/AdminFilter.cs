using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data;
using System.Linq;

namespace NewRefApp.Middlewares
{
    public class AdminFilter : IAuthorizationFilter
    {
        private readonly ApplicationDbContext _context;

        public AdminFilter(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            var rareentAdmin = request.Cookies["rareentAdmin"];
            var findUserCookie = request.Cookies["findUser"]; // reuse same cookie

            bool isAdmin = false;

            if (string.IsNullOrEmpty(findUserCookie))
            {
                var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == rareentAdmin);
                if (user != null && user.IsAdmin)
                {
                    isAdmin = true;

                    // Set cookie
                    response.Cookies.Append("findUser", "true", new CookieOptions
                    {
                        Path = "/",
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });
                }
            }
            else
            {
                bool.TryParse(findUserCookie, out isAdmin);
            }

            if (string.IsNullOrEmpty(rareentAdmin) || !isAdmin)
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                return;
            }

            Console.WriteLine($"[AdminFilter] Authenticated admin: {rareentAdmin}");
        }
    }
}

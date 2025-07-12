using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace NewRefApp.Middlewares
{
    public class UserFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var rareentUser = request.Cookies["rareentUser"];

            if (string.IsNullOrEmpty(rareentUser))
            {
                if (rareentUser == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Authorization", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
                // Not authorized, redirect to login page
                //context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            else
            {
                // Optional: Validate rareentUser in your database here
                Console.WriteLine($"Authenticated rareentUser: {rareentUser}");
            }
        }

    }
}

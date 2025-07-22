using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace NewRefApp.Middlewares
{
    public class AdminFilter :IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var rareentAdmin = request.Cookies["rareentAdmin"];

            if (string.IsNullOrEmpty(rareentAdmin))
            {
                if (rareentAdmin == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Admin", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Admin", null);
                }
                // Not authorized, redirect to login page
                //context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            else
            {
                // Optional: Validate rareentAdmin in your database here
                Console.WriteLine($"Authenticated rareentAdmin: {rareentAdmin}");
            }
        }

    }
}

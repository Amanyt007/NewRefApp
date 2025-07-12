using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
        ViewBag.IsLoggedIn = isLoggedIn;
        ViewBag.PhoneNumber = HttpContext.Session.GetString("UserPhone");

        base.OnActionExecuting(context);
    }
}

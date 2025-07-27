using NewRefApp.Services;

namespace NewRefApp.Middlewares
{
    public class UserResolverMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserResolverMiddleware> _logger;

        public UserResolverMiddleware(RequestDelegate next, ILogger<UserResolverMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var rareentUser = context.Request.Cookies["rareentUser"];
                var rareentAdmin = context.Request.Cookies["rareentAdmin"];

                // ✅ Resolve scoped service inside the request
                var userService = context.RequestServices.GetService<IUserService>();

                if (!string.IsNullOrEmpty(rareentUser))
                {
                    var user = await userService.GetByPhoneAsync(rareentUser);
                    if (user != null)
                    {
                        context.Items["CurrentUser"] = user;
                    }
                }
                else if (!string.IsNullOrEmpty(rareentAdmin))
                {
                    var admin = await userService.GetByPhoneAsync(rareentAdmin);
                    if (admin != null && admin.IsAdmin)
                    {
                        context.Items["CurrentUser"] = admin;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve user.");
            }

            await _next(context);
        }
    }
}

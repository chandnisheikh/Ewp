using Microsoft.AspNetCore.Http;

namespace Ewp.Helpers
{
    public static class AuthHelper
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            return context.Session.GetString("Username") != null;
        }

        public static string GetRole(HttpContext context)
        {
            return context.Session.GetString("Role") ?? "";
        }
    }
}
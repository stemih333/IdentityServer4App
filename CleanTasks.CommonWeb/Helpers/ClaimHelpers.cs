using System.Linq;
using System.Security.Claims;

namespace CleanTasks.CommonWeb.Helpers
{
    public static class ClaimHelpers
    {
        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(_ => _.Type.Equals("name"))?.Value;
        }
    }
}

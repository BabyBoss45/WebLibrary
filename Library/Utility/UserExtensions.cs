using System.Security.Claims;
using System.Security.Principal;

namespace Library
{
    public static class UserExtensions
    {
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (!principal.Identity.IsAuthenticated)
                return 0;

            return long.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public static bool IsInAnyRole(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(principal.IsInRole);
        }

        public static bool IsOnlyRole(this ClaimsPrincipal principal, string role)
        {
            var userRoles = ((ClaimsIdentity)principal.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return userRoles.Count() == 1 && userRoles.Contains(role);
        }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace Library
{   
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
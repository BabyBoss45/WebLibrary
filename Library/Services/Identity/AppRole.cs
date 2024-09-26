using Microsoft.AspNetCore.Identity;

namespace Library.Services.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public AppRole() { }
        public AppRole(string roleName)
            : base(roleName)
        { }
    }
}

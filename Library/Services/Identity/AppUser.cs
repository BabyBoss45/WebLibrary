using System;
using Microsoft.AspNetCore.Identity;

namespace Library
{
    public class AppUser : IdentityUser<long>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}

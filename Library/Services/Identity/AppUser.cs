using System;
using Microsoft.AspNetCore.Identity;

namespace Library
{
    public class AppUser : IdentityUser<long>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Department { get; set; }
        public string Country { get; set; }
    }
}
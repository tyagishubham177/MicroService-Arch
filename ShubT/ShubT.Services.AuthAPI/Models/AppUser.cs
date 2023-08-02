using Microsoft.AspNetCore.Identity;

namespace ShubT.Services.AuthAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
    }
}

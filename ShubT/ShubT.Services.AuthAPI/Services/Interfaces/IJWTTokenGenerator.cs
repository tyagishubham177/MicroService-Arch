using ShubT.Services.AuthAPI.Models;

namespace ShubT.Services.AuthAPI.Services.Interfaces
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(AppUser user, IEnumerable<string> roles);
    }
}

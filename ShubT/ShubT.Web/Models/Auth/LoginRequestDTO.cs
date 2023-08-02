using System.ComponentModel.DataAnnotations;

namespace ShubT.Web.Models.Auth
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

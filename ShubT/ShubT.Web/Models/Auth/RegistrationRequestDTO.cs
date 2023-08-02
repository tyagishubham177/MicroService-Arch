using System.ComponentModel.DataAnnotations;

namespace ShubT.Web.Models.Auth
{
    public class RegistrationRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}

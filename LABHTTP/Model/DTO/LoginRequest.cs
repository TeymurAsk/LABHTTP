using System.ComponentModel.DataAnnotations;

namespace LABHTTP.Model.DTO
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(11)]
        public string Password { get; set; } = null!;
    }
}

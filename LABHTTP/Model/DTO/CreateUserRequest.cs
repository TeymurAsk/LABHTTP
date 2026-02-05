using System.ComponentModel.DataAnnotations;

namespace LABHTTP.Model.DTO
{
    public class CreateUserRequest
    {

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(11)]
        public string Password { get; set; } = null!;
    }
}

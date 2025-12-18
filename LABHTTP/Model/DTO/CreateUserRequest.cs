using System.ComponentModel.DataAnnotations;

namespace LABHTTP.Model.DTO
{
    public class CreateUserRequest
    {

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

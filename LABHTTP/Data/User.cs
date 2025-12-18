using System.ComponentModel.DataAnnotations;

namespace LABHTTP.Data
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}

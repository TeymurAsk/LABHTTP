using System.ComponentModel.DataAnnotations;

namespace LABHTTP.Model.DTO
{
    public class NoteRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = null!;
    }
}

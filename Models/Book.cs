using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [StringLength(100)]
        public string? Author { get; set; }

        [Required]
        [StringLength(13)]
        public string? ISBN { get; set; }
    }
}

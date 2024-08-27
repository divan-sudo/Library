using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Email { get; set; }
    }
}

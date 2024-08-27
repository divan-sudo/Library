using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Loan : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}

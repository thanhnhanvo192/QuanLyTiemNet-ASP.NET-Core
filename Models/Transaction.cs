using System.ComponentModel.DataAnnotations;

namespace QuanLyTiemNET.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int ComputerID { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public int Status { get; set; } = 1;
        public DateTime TransactionTime { get; set; }
    }
}

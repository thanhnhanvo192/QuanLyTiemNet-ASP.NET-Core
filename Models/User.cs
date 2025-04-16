using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTiemNET.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AccountBalance { get; set; }
        [Required]
        public string Role { get; set; } = "User";
        public int Status { get; set; } = 1;
    }
}

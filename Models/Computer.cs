using System.ComponentModel.DataAnnotations;

namespace QuanLyTiemNET.Models
{
    public class Computer
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Status { get; set; } = "Có sẵn";
        [Required]
        public decimal HourlyRate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QuanLyTiemNET.Models
{
    public enum ComputerStatus
    {
        Unknown = 0,

        [Display(Name = "Bảo trì")]
        Maintenance = 1,

        [Display(Name = "Có sẵn")]
        Available = 2,

        [Display(Name = "Đang sử dụng")]
        InUse = 3
    }
    public class Computer
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public ComputerStatus Status { get; set; }
        [Required]              
        public decimal HourlyRate { get; set; }
    }
}

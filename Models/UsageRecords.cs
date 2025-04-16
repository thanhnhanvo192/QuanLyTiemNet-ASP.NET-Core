using System.ComponentModel.DataAnnotations;

namespace QuanLyTiemNET.Models
{
    public class UsageRecords
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int ComputerID { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } = null;
    }
}

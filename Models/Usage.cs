namespace QuanLyTiemNET.Models
{
    public class Usage
    {
        public int UsageID { get; set; }
        public int UserID { get; set; }
        public int ComputerID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public User User { get; set; }
        public Computer Computer { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using QuanLyTiemNET.Models;

namespace QuanLyTiemNET
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; } // Bảng người dùng
        public DbSet<Computer> Computers { get; set; } // Bảng máy tính
        public DbSet<UsageRecords> UsageRecords { get; set; } // Bảng hóa đơn
        public DbSet<Transaction> Transactions { get; set; } // Bảng giao dịch
    }
}
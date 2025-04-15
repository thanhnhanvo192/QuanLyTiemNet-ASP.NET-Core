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
        public DbSet<Usage> Usages { get; set; } // Bảng hóa đơn
    }
}
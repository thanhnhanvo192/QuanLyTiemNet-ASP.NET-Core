using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTiemNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Đặt Cookie là scheme mặc định
            .AddCookie(options => // Đăng ký handler xử lý Cookie
            {
                // Các cấu hình quan trọng cho Cookie handler
                options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn
                options.SlidingExpiration = true;
                // options.AccessDeniedPath = "/Account/AccessDenied"; // Tùy chọn
                // options.LogoutPath = "/Account/Logout"; // Tùy chọn
                // options.Cookie.Name = ".YourApp.AuthCookie"; // Tùy chọn đặt tên cookie
            });

            var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

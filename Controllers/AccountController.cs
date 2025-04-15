using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTiemNET.Models;
namespace QuanLyTiemNET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Nên giữ lại
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null) // Đảm bảo dùng ViewModel (ví dụ: LoginViewModel)
        {
            // ModelState.IsValid nên được kiểm tra ở đây nếu dùng ViewModel với DataAnnotations
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về lỗi validation nếu có
            }

            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower());

            // Kiểm tra user tồn tại, hoạt động và mật khẩu đúng (so sánh text gốc)
            if (user != null && user.Status == 1 && model.Password == user.Password)
            {
                // --- BẮT ĐẦU PHẦN THÊM VÀO ĐỂ THỰC SỰ ĐĂNG NHẬP ---
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Name, user.Username), // Quan trọng cho User.Identity.Name
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            // Thêm các claim khác nếu cần
        };

                // Tạo ClaimsIdentity VỚI AuthenticationScheme
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // IsPersistent = model.RememberMe, // Cho phép "Ghi nhớ đăng nhập" nếu có
                    // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // Ví dụ: đặt thời gian hết hạn
                };

                // *** Gọi SignInAsync để tạo cookie xác thực ***
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                // --- KẾT THÚC PHẦN THÊM VÀO ---

                // Bây giờ mới chuyển hướng
                return RedirectToAction("Index", "Home");
            }
            else // Gộp các trường hợp thất bại
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa.");
            }

            // Trả về view nếu ModelState không hợp lệ ban đầu hoặc đăng nhập thất bại
            return View(model);
        }
        [HttpPost] // Use POST for logout
        [ValidateAntiForgeryToken] // Protect against CSRF
        public async Task<IActionResult> Logout()
        {
            // Sign out using the same scheme used for signing in
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to a public page (e.g., Home Page)
            return RedirectToAction("Index", "Home");
            // Or redirect to the Login page:
            // return RedirectToAction("Login", "Account");
        }
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTiemNET.Models;

namespace QuanLyTiemNET.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var transactions = await _context.Transactions
                .Where(t => t.UserID == currentUserId && t.Status == 1)
                .OrderByDescending(t => t.TransactionTime)
                .ToListAsync();

            return View(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(int id)
        {
            // Lấy user hiện tại
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
                return RedirectToAction("Login", "Account");

            // Lấy transaction theo id
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserID == currentUserId && t.Status == 1);

            if (transaction == null)
            {
                TempData["Message"] = "Không tìm thấy giao dịch hoặc giao dịch đã thanh toán.";
                return RedirectToAction("Index");
            }

            // Lấy user
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            // Kiểm tra số dư
            if (user.AccountBalance >= transaction.Amount)
            {
                user.AccountBalance -= transaction.Amount;
                transaction.Status = 0;
                _context.Users.Update(user);
                _context.Transactions.Update(transaction);

                await _context.SaveChangesAsync();
                TempData["Message"] = "Thanh toán thành công!";
            }
            else
            {
                TempData["Message"] = "Số dư không đủ. Vui lòng nạp thêm tiền.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Statistic()
        {
            var result = await _context.Transactions
                .GroupBy(t => t.ComputerID)
                .Select(g => new StatisticViewModel
                {
                    ComputerID = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            return View(result);
        }

        public IActionResult DepositCash()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DepositCash(DepositCashViewModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
            {
                TempData["Message"] = "Không thể xác định người dùng.";
                return View(model);
            }
            if (!int.TryParse(userIdStr, out int userId))
            {
                TempData["Message"] = "ID người dùng không hợp lệ.";
                return View(model);
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng.";
                return View(model);
            }
            user.AccountBalance += model.Amount;
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Nạp thành công {model.Amount:N0}đ vào tài khoản!";
            return RedirectToAction("DepositCash");
        }
    }
}

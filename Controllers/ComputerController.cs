using System.Security.Claims;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTiemNET.Models;

namespace QuanLyTiemNET.Controllers
{
    public class ComputerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ComputerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var computers = _context.Computers.Where(c => c.Status != ComputerStatus.Unknown).ToList();
            return View("Index", computers);
        }

        // GET: Computer/Details/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            var computer = await _context.Computers.FirstOrDefaultAsync(m => m.id == id);
            if (computer == null) return NotFound();
            return View(computer);
        }


        // GET: Computer/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Computer/Create
        [HttpPost]
        public IActionResult Create(Computer computer)
        {
            if (ModelState.IsValid)
            {
                _context.Computers.Add(computer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(computer);
        }


        // GET: Computer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var computer = await _context.Computers.FindAsync(id);
            if (computer == null) return NotFound();

            return View(computer);
        }
        // POST: Computer/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Computer computer)
        {
            if (computer == null) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingComputer = await _context.Computers.AsNoTracking().FirstOrDefaultAsync(u => u.id == id);
                    if (existingComputer == null) return NotFound();
                    _context.Computers.Update(computer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }
            return View(computer);
        }

        // GET: Computer/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var computer = await _context.Computers.FirstOrDefaultAsync(m => m.id == id);
            if (computer == null) return NotFound();

            return View(computer);
        }
        // POST: Computer/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var computer = await _context.Computers.FindAsync(id);
            if (computer == null) return NotFound();
            computer.Status = ComputerStatus.Unknown;
            _context.Computers.Update(computer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // POST: Computer/Select
        [HttpPost]
        public async Task<IActionResult> SelectComputer(int id)
        {
            var computer = await _context.Computers.FindAsync(id);
            if (computer == null) return NotFound();
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }
            var now = DateTime.UtcNow;
            var usageRecord = new UsageRecords
            {
                UserID = currentUserId,
                ComputerID = computer.id, // id từ tham số action
                StartTime = now,
                EndTime = null // EndTime ban đầu là null
            };
            _context.UsageRecords.Add(usageRecord);
            computer.Status = ComputerStatus.InUse;
            _context.Computers.Update(computer);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Bạn đã chọn máy tính có ID {id} thành công!";
            TempData["SelectedComputerId"] = id;
            return RedirectToAction("Index", "Home");
        }

        // POST: Computer/Unselect
        [HttpPost]
        public async Task<IActionResult> UnselectComputer()
        {
            int? computerId = TempData["SelectedComputerId"] as int?;
            var computerToUpdate = await _context.Computers.FindAsync(computerId);
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int currentUserId))
            {
                var now = DateTime.UtcNow;

                var activeUsageRecord = await _context.UsageRecords
                                                      .OrderByDescending(r => r.StartTime)
                                                      .FirstOrDefaultAsync(r => r.UserID == currentUserId && r.EndTime == null);

                if (activeUsageRecord != null)
                {
                    // Cập nhật thời gian kết thúc
                    activeUsageRecord.EndTime = now;

                    // Tính toán thời gian sử dụng và chi phí
                    var duration = now - activeUsageRecord.StartTime;
                    decimal totalCost = 0;

                    if (computerToUpdate == null)
                    {
                        computerToUpdate = await _context.Computers.FindAsync(activeUsageRecord.ComputerID);
                    }

                    if (computerToUpdate != null)
                    {
                        totalCost = (decimal)duration.TotalMinutes * (computerToUpdate.HourlyRate / 60.0m);

                        if (computerToUpdate.Status == ComputerStatus.InUse)
                        {
                            computerToUpdate.Status = ComputerStatus.Available;
                            _context.Computers.Update(computerToUpdate);
                        }
                    }

                    // Thêm bản ghi giao dịch
                    var transaction = new Transaction
                    {
                        UserID = currentUserId,
                        ComputerID = computerToUpdate.id,
                        Amount = totalCost,
                        TransactionTime = now
                    };

                    _context.Transactions.Add(transaction);
                    _context.UsageRecords.Update(activeUsageRecord);

                    try
                    {
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"Bạn đã trả máy thành công. Tổng tiền: {totalCost:N0} đ.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Lỗi khi lưu thông tin: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            TempData["Message"] += " | Inner: " + ex.InnerException.Message;
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

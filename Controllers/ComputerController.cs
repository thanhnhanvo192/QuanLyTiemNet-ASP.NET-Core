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
            var computers = _context.Computers.Where(c => c.Status != "0").ToList();
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
            computer.Status = "0";
            _context.Computers.Update(computer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

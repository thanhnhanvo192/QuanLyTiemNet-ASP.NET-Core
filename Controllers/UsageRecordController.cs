using Microsoft.AspNetCore.Mvc;

namespace QuanLyTiemNET.Controllers
{
    public class UsageRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsageRecordController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

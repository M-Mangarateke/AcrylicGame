using AcrylicGame.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Services
        public async Task<IActionResult> Index()
        {
            var branches = await _context.Branches
                .OrderBy(b => b.Name)
                .ToListAsync();

            ViewBag.Branches = branches;
            return View();
        }
    }
}


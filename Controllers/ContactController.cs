using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcrylicGame.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessage message)
        {
            if (ModelState.IsValid)
            {
                message.SentAt = DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thanks! We'll get back to you shortly.";
                return RedirectToAction(nameof(Index));
            }

            return View(message);
        }
    }

}

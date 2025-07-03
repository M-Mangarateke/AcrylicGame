using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /ClientDashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Branch)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // POST: /ClientDashboard/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == user.Id);

            if (booking == null || booking.Status != "Pending")
                return BadRequest("Only pending bookings can be cancelled.");

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Warning"] = "Booking cancelled.";
            return RedirectToAction("Index");
        }
    }
}

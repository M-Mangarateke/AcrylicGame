using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff == null || staff.BranchId == null)
                return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Branch)
                .Where(b => b.BranchId == staff.BranchId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var testimonials = await _context.Testimonials
                .Where(t => !t.IsApproved && t.BranchId == staff.BranchId) // Ensure branch filter for staff
                .OrderByDescending(t => t.SubmittedAt)
                .ToListAsync();

            ViewBag.Testimonials = testimonials;

            return View(bookings);
        }

        // POST: StaffDashboard/ConfirmBooking/5
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.IsConfirmedByStaff = true;
            booking.Status = "Confirmed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking confirmed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffBookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffBookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /StaffBooking
        public async Task<IActionResult> Index(string statusFilter = "Pending")
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff == null || staff.BranchId == null)
                return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Branch)
                .Where(b => b.BranchId == staff.BranchId && b.Status == statusFilter)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            return View(bookings);
        }

        // GET: /StaffBooking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null) return Unauthorized();

            var booking = await _context.Bookings
                .Include(b => b.Branch)
                .FirstOrDefaultAsync(b => b.Id == id && b.BranchId == staff.BranchId);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: /StaffBooking/Confirm/5
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null) return Unauthorized();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.BranchId == staff.BranchId);

            if (booking == null) return NotFound();

            booking.Status = "Confirmed";
            booking.IsConfirmedByStaff = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmed.";
            return RedirectToAction("Index");
        }

        // POST: /StaffBooking/Reject/5
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null) return Unauthorized();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.BranchId == staff.BranchId);

            if (booking == null) return NotFound();

            booking.Status = "Rejected";
            booking.IsConfirmedByStaff = false;
            await _context.SaveChangesAsync();

            TempData["Warning"] = "Booking rejected.";
            return RedirectToAction("Index");
        }
    }
}

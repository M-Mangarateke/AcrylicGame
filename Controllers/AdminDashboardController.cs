using AcrylicGame.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalBookings = await _context.Bookings.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalPromotions = await _context.Promotions.CountAsync();

            ViewBag.TotalBookings = totalBookings;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalPromotions = totalPromotions;

            return View();
        }
    }
}

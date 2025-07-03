using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    [Authorize(Roles = "Client")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Booking/Create
        public async Task<IActionResult> Create()
        {
            var branches = await _context.Branches
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            var viewModel = new BookingFormViewModel
            {
                Branches = new SelectList(branches, "Id", "Name"),
                BookingDate = DateTime.Today // default to today
            };

            return View(viewModel);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var branches = await _context.Branches
                    .Select(b => new { b.Id, b.Name })
                    .ToListAsync();
                model.Branches = new SelectList(branches, "Id", "Name");

                
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                
                return RedirectToAction("Index", "Home");
            }

            // Upload proof of payment file (accepts image and PDF)
            string proofPath = null;
            if (model.ProofOfPayment != null && model.ProofOfPayment.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var fileExt = Path.GetExtension(model.ProofOfPayment.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExt))
                {
                    ModelState.AddModelError("ProofOfPayment", "Invalid file type. Only JPG, PNG, and PDF are allowed.");
                    return View(model);
                }

                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "payments");
                Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid() + fileExt;
                var fullPath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ProofOfPayment.CopyToAsync(stream);
                }

                proofPath = Path.Combine("/uploads/payments", fileName);
            }

            var booking = new Booking
            {
                UserId = user.Id,
                CustomerName = model.CustomerName,
                Email = model.Email,
                Phone = model.Phone,
                BookingDate = model.BookingDate.Date, // saves as midnight
                BranchId = model.BranchId,
                ServiceDescription = model.ServiceDescription,
                ProofOfPaymentPath = proofPath,
                Status = "Pending",
                IsConfirmedByStaff = false
            };

            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                
                return RedirectToAction("Index", "ClientDashboard");
            }
            catch (Exception ex)
            {
                
                return RedirectToAction("Create");
            }
        }
    }
}


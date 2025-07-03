using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PromotionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Promotions
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions
                .Include(p => p.Branch)
                .Where(p => !p.IsArchived)
                .OrderByDescending(p => p.ValidFrom)
                .ToListAsync();

            return View(promotions);
        }

        // GET: /Promotions/Create
        [Authorize(Roles = "Staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Promotions/Create
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null)
                return Unauthorized();

            // Save image
            string imagePath = string.Empty;
            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "promotions");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);
                imagePath = Path.Combine("/uploads/promotions", fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.Image.CopyToAsync(stream);
            }

            var promotion = new Promotion
            {
                Title = model.Title,
                Description = model.Description,
                ImagePath = imagePath,
                ValidFrom = model.ValidFrom,
                ValidTo = model.ValidTo,
                BranchId = staff.BranchId.Value,
                IsArchived = false
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Promotion posted successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Promotions/Archive/5
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Archive(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null)
                return Unauthorized();

            var promo = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Id == id && p.BranchId == staff.BranchId);

            if (promo == null) return NotFound();

            promo.IsArchived = true;
            await _context.SaveChangesAsync();

            TempData["Info"] = "Promotion archived.";
            return RedirectToAction("Index");
        }
    }
}

using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public GalleryController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // GET: /Gallery
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? branchId)
        {
            var query = _context.GalleryItems
                .Include(g => g.Branch)
                .AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(g => g.BranchId == branchId.Value);
            }

            var items = await query
                .OrderByDescending(g => g.UploadedAt)
                .ToListAsync();

            ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
            ViewBag.SelectedBranchId = branchId;

            return View(items);
        }

        // GET: /Gallery/Upload
        [Authorize(Roles = "Staff")]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Gallery/Upload
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(GalleryUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null) return Unauthorized();

            // Save image
            string imagePath = string.Empty;
            if (model.Image != null)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "gallery");
                Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var fullPath = Path.Combine(uploadDir, fileName);
                imagePath = Path.Combine("/uploads/gallery", fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.Image.CopyToAsync(stream);
            }

            var galleryItem = new GalleryItem
            {
                BranchId = staff.BranchId.Value,
                Caption = model.Caption,
                ImagePath = imagePath,
                UploadedAt = DateTime.Now
            };

            _context.GalleryItems.Add(galleryItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Photo uploaded successfully.";
            return RedirectToAction("Index");
        }
    }
}

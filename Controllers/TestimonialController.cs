using AcrylicGame.Data;
using AcrylicGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AcrylicGame.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestimonialController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Testimonial
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? branchId)
        {
            var testimonials = _context.Testimonials
                .Include(t => t.Branch)
                .Where(t => t.IsApproved);

            if (branchId.HasValue)
            {
                testimonials = testimonials.Where(t => t.BranchId == branchId);
            }

            var branches = await _context.Branches
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            ViewBag.SelectedBranchId = branchId;

            var list = await testimonials.OrderByDescending(t => t.SubmittedAt).ToListAsync();
            return View(list);
        }


        // GET: /Testimonial/Create
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            var branches = await _context.Branches
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            return View();
        }

        // POST: /Testimonial/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestimonialFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "You must be logged in to submit a testimonial.";
                return RedirectToAction("Index");
            }

            var testimonial = new Testimonial
            {
                CustomerName = model.CustomerName,
                Message = model.Message,
                BranchId = model.BranchId,
                SubmittedAt = DateTime.Now,
                UserId = user.Id,
                IsApproved = false
            };

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            
            return RedirectToAction("Index");
        }

        // GET: /Testimonial/Pending (staff only)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Pending()
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null)
                return Unauthorized();

            var pending = await _context.Testimonials
                .Include(t => t.Branch)
                .Where(t => t.BranchId == staff.BranchId && !t.IsApproved)
                .OrderByDescending(t => t.SubmittedAt)
                .ToListAsync();

            return View(pending);
        }

        // POST: /Testimonial/Approve/5
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Approve(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null)
                return Unauthorized();

            var testimonial = await _context.Testimonials
                .FirstOrDefaultAsync(t => t.Id == id && t.BranchId == staff.BranchId);

            if (testimonial == null) return NotFound();

            testimonial.IsApproved = true;
            await _context.SaveChangesAsync();

            
            return RedirectToAction("Pending");
        }

        // POST: /Testimonial/Delete/5
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _userManager.GetUserAsync(User);
            if (staff?.BranchId == null)
                return Unauthorized();

            var testimonial = await _context.Testimonials
                .FirstOrDefaultAsync(t => t.Id == id && t.BranchId == staff.BranchId);

            if (testimonial == null) return NotFound();

            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();

            
            return RedirectToAction("Pending");
        }
    }
}


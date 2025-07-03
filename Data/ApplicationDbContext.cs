using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AcrylicGame.Models;

namespace AcrylicGame.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Booking: One user -> many bookings
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking: One branch -> many bookings (fixed WithMany to specify Bookings navigation property)
            builder.Entity<Booking>()
                .HasOne(b => b.Branch)
                .WithMany(br => br.Bookings)
                .HasForeignKey(b => b.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Testimonial: One user -> many testimonials
            builder.Entity<Testimonial>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Testimonial: One branch -> many testimonials
            builder.Entity<Testimonial>()
                .HasOne(t => t.Branch)
                .WithMany(br => br.Testimonials)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser: optional branch for staff
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany()
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Promotion: One branch -> many promotions
            builder.Entity<Promotion>()
                .HasOne(p => p.Branch)
                .WithMany(br => br.Promotions)
                .HasForeignKey(p => p.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // GalleryItem: One branch -> many gallery items
            builder.Entity<GalleryItem>()
                .HasOne(g => g.Branch)
                .WithMany(br => br.GalleryItems)
                .HasForeignKey(g => g.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter for promotions
            builder.Entity<Promotion>()
                .HasQueryFilter(p => !p.IsArchived);

            // Removed global auto-approve filter for testimonials
            // Always filter with .Where(t => t.IsApproved) when querying for public testimonials
        }
    }
}

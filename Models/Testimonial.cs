using System;
using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class Testimonial
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string CustomerName { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}

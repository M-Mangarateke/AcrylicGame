using System;
using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public string ServiceDescription { get; set; } // user-entered service note

        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        public string? ProofOfPaymentPath { get; set; }  // uploaded file path
        public bool IsConfirmedByStaff { get; set; } = false;
    }
}


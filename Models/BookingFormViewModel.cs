using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class BookingFormViewModel
    {
        [Required]
        public string CustomerName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please describe what you would like to do.")]
        [Display(Name = "What would you like to do?")]
        public string ServiceDescription { get; set; }

        [Required(ErrorMessage = "Proof of payment is required")]
        [Display(Name = "Proof of Payment (50% Deposit)")]
        public IFormFile ProofOfPayment { get; set; }

        public SelectList Branches { get; set; }
    }
}

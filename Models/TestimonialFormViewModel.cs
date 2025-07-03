using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class TestimonialFormViewModel
    {
        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}

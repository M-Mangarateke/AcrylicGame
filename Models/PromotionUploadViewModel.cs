using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class PromotionUploadViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        [DataType(DataType.Date)]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Display(Name = "Valid To")]
        [DataType(DataType.Date)]
        public DateTime ValidTo { get; set; }
    }
}

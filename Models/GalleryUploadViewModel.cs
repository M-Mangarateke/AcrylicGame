using System.ComponentModel.DataAnnotations;

namespace AcrylicGame.Models
{
    public class GalleryUploadViewModel
    {
        [Required]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = "Caption")]
        public string Caption { get; set; }
    }
}

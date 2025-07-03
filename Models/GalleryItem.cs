using System;

namespace AcrylicGame.Models
{
    public class GalleryItem
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Caption { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}


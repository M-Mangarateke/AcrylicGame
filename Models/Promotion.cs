using System;

namespace AcrylicGame.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public bool IsArchived { get; set; } = false; // for soft delete/archive
    }
}


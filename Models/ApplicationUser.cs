using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AcrylicGame.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public int? BranchId { get; set; }  // Null for public users
        public Branch? Branch { get; set; }

    }
}

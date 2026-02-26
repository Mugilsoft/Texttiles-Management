using Microsoft.AspNetCore.Identity;
using System;

namespace TextileBilling.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}

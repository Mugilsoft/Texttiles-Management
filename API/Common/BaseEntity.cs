using System;

namespace TextileBilling.Core.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "System";
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; } = "System";
        public bool IsDeleted { get; set; } = false;
    }
}

using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public class Counter : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

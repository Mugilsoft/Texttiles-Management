using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public class Stock : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public double Quantity { get; set; }
        public double MinStockLevel { get; set; }
    }
}

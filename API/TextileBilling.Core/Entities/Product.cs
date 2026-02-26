using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Fabric { get; set; } = string.Empty;
        
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal MRP { get; set; }
        
        public decimal TaxPercentage { get; set; } // GST
        public bool IsActive { get; set; } = true;
    }
}

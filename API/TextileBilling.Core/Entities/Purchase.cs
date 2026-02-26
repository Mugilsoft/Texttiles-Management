using System;
using System.Collections.Generic;
using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetAmount { get; set; }

        public string Status { get; set; } // Pending, Received, Cancelled
        public string Remarks { get; set; }

        public virtual ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }

    public class PurchaseItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
    }
}

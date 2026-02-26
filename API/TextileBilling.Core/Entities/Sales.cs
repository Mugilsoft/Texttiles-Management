using System;
using System.Collections.Generic;
using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public class SalesInvoice : BaseEntity
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public int CounterId { get; set; }
        public virtual Counter Counter { get; set; }
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string CreatedByUserId { get; set; }
        public virtual ApplicationUser CreatedByUser { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }

        public string PaymentMode { get; set; } // Cash, Card, UPI, etc.
        public string Remarks { get; set; }

        public virtual ICollection<SalesItem> Items { get; set; } = new List<SalesItem>();
    }

    public class SalesItem : BaseEntity
    {
        public int SalesInvoiceId { get; set; }
        public virtual SalesInvoice SalesInvoice { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }
}

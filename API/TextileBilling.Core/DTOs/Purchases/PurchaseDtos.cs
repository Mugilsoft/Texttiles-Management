using System;
using System.Collections.Generic;

namespace TextileBilling.Core.DTOs.Purchases
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new List<PurchaseItemDto>();
    }

    public class PurchaseItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
    }
}

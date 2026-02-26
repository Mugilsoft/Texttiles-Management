using System;
using System.Collections.Generic;

namespace TextileBilling.Core.DTOs.Sales
{
    public class SalesInvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int CounterId { get; set; }
        public string CounterName { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }

        public string PaymentMode { get; set; }
        public string Remarks { get; set; }

        public List<SalesItemDto> Items { get; set; } = new List<SalesItemDto>();
    }

    public class SalesItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }
}

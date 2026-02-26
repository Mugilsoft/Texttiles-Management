using System;
using System.Collections.Generic;

namespace TextileBilling.Core.DTOs.Reports
{
    public class DayBookDto
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalTaxCollected { get; set; }
        public decimal TotalTaxPaid { get; set; }
        public decimal NetCashFlow { get; set; }
        
        public List<PaymentSummaryDto> SalesByPaymentMode { get; set; } = new List<PaymentSummaryDto>();
    }

    public class PaymentSummaryDto
    {
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
    }

    public class SalesReportDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public double TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalTax { get; set; }
    }

    public class PurchaseReportDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public double TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalTax { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Reports;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DayBookDto> GetDayBookAsync(DateTime date, int? branchId)
        {
            var targetDate = date.Date;
            
            var salesQuery = _context.SalesInvoices.Where(s => s.InvoiceDate.Date == targetDate);
            var purchaseQuery = _context.PurchaseOrders.Where(p => p.PODate.Date == targetDate);
            
            if (branchId.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.BranchId == branchId.Value);
                purchaseQuery = purchaseQuery.Where(p => p.BranchId == branchId.Value);
            }

            var sales = await salesQuery.ToListAsync();
            var purchases = await purchaseQuery.ToListAsync();

            var dto = new DayBookDto
            {
                Date = targetDate,
                TotalSales = sales.Sum(s => s.NetAmount),
                TotalPurchases = purchases.Sum(p => p.NetAmount),
                TotalTaxCollected = sales.Sum(s => s.TotalTax),
                TotalTaxPaid = purchases.Sum(p => p.TotalTax),
                NetCashFlow = sales.Sum(s => s.NetAmount) - purchases.Sum(p => p.NetAmount)
            };

            dto.SalesByPaymentMode = sales
                .GroupBy(s => s.PaymentMode)
                .Select(g => new PaymentSummaryDto 
                { 
                    PaymentMode = g.Key ?? "Unknown", 
                    Amount = g.Sum(x => x.NetAmount) 
                }).ToList();

            return dto;
        }

        public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(DateTime from, DateTime to, int? branchId)
        {
            var query = _context.SalesItems
                .Include(i => i.SalesInvoice)
                .Include(i => i.Product)
                .Where(i => i.SalesInvoice.InvoiceDate >= from && i.SalesInvoice.InvoiceDate <= to);

            if (branchId.HasValue)
            {
                query = query.Where(i => i.SalesInvoice.BranchId == branchId.Value);
            }

            return await query
                .GroupBy(i => new { i.ProductId, i.Product.Name, i.Product.SKU })
                .Select(g => new SalesReportDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    SKU = g.Key.SKU,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.SubTotal),
                    TotalTax = g.Sum(x => x.TaxAmount)
                }).ToListAsync();
        }

        public async Task<IEnumerable<PurchaseReportDto>> GetPurchaseReportAsync(DateTime from, DateTime to, int? branchId)
        {
            var query = _context.PurchaseItems
                .Include(i => i.PurchaseOrder)
                .Include(i => i.Product)
                .Where(i => i.PurchaseOrder.PODate >= from && i.PurchaseOrder.PODate <= to);

            if (branchId.HasValue)
            {
                query = query.Where(i => i.PurchaseOrder.BranchId == branchId.Value);
            }

            return await query
                .GroupBy(i => new { i.ProductId, i.Product.Name, i.Product.SKU })
                .Select(g => new PurchaseReportDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    SKU = g.Key.SKU,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalCost = g.Sum(x => x.SubTotal),
                    TotalTax = g.Sum(x => x.TaxAmount)
                }).ToListAsync();
        }
    }
}

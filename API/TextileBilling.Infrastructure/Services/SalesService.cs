using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Sales;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.Infrastructure.Services
{
    public class SalesService : ISalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockService _stockService;

        public SalesService(ApplicationDbContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task<SalesInvoiceDto> CreateSalesAsync(SalesInvoiceDto dto)
        {
            var invoice = new SalesInvoice
            {
                InvoiceNumber = $"INV-{DateTime.Now.Ticks}",
                InvoiceDate = DateTime.Now,
                BranchId = dto.BranchId,
                CounterId = dto.CounterId,
                CustomerId = dto.CustomerId,
                CreatedByUserId = dto.CreatedByUserId,
                TotalAmount = dto.TotalAmount,
                TotalTax = dto.TotalTax,
                Discount = dto.Discount,
                NetAmount = dto.NetAmount,
                PaymentMode = dto.PaymentMode,
                Remarks = dto.Remarks
            };

            foreach (var item in dto.Items)
            {
                invoice.Items.Add(new SalesItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxPercentage = item.TaxPercentage,
                    TaxAmount = item.TaxAmount,
                    Discount = item.Discount,
                    SubTotal = item.SubTotal
                });

                // Update Stock
                await _stockService.UpdateStockAsync(
                    item.ProductId,
                    dto.BranchId,
                    item.Quantity,
                    TransactionType.Sale,
                    invoice.InvoiceNumber,
                    $"Sale via POS: {invoice.InvoiceNumber}"
                );
            }

            _context.SalesInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            dto.Id = invoice.Id;
            dto.InvoiceNumber = invoice.InvoiceNumber;
            dto.InvoiceDate = invoice.InvoiceDate;
            
            return dto;
        }

        public async Task<IEnumerable<SalesInvoiceDto>> GetAllSalesAsync()
        {
            return await _context.SalesInvoices
                .Include(s => s.Branch)
                .Include(s => s.Counter)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.InvoiceDate)
                .Select(s => new SalesInvoiceDto
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    InvoiceDate = s.InvoiceDate,
                    BranchId = s.BranchId,
                    BranchName = s.Branch.Name,
                    CounterId = s.CounterId,
                    CounterName = s.Counter.Name,
                    CustomerId = s.CustomerId,
                    CustomerName = s.Customer != null ? s.Customer.Name : "Walk-in",
                    TotalAmount = s.TotalAmount,
                    NetAmount = s.NetAmount,
                    PaymentMode = s.PaymentMode
                }).ToListAsync();
        }

        public async Task<SalesInvoiceDto> GetSalesByIdAsync(int id)
        {
            var s = await _context.SalesInvoices
                .Include(s => s.Branch)
                .Include(s => s.Counter)
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null) return null;

            return new SalesInvoiceDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                InvoiceDate = s.InvoiceDate,
                BranchId = s.BranchId,
                BranchName = s.Branch.Name,
                CounterId = s.CounterId,
                CounterName = s.Counter.Name,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer != null ? s.Customer.Name : "Walk-in",
                TotalAmount = s.TotalAmount,
                TotalTax = s.TotalTax,
                Discount = s.Discount,
                NetAmount = s.NetAmount,
                PaymentMode = s.PaymentMode,
                Remarks = s.Remarks,
                Items = s.Items.Select(i => new SalesItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    SKU = i.Product.SKU,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TaxPercentage = i.TaxPercentage,
                    TaxAmount = i.TaxAmount,
                    Discount = i.Discount,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }
    }
}

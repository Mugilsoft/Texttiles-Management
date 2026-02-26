using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Purchases;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.Infrastructure.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockService _stockService;

        public PurchaseService(ApplicationDbContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(PurchaseOrderDto dto)
        {
            var po = new PurchaseOrder
            {
                PONumber = dto.PONumber ?? $"PO-{DateTime.Now.Ticks}",
                PODate = dto.PODate == default ? DateTime.Now : dto.PODate,
                VendorId = dto.VendorId,
                BranchId = dto.BranchId,
                Status = "Pending",
                Remarks = dto.Remarks,
                TotalAmount = dto.Items.Sum(x => (decimal)x.Quantity * x.UnitPrice),
                TotalTax = dto.Items.Sum(x => x.TaxAmount),
                NetAmount = dto.Items.Sum(x => x.SubTotal)
            };

            foreach (var item in dto.Items)
            {
                po.Items.Add(new PurchaseItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxPercentage = item.TaxPercentage,
                    TaxAmount = item.TaxAmount,
                    SubTotal = item.SubTotal
                });
            }

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            dto.Id = po.Id;
            dto.PONumber = po.PONumber;
            return dto;
        }

        public async Task<bool> ReceivePurchaseOrderAsync(int id)
        {
            var po = await _context.PurchaseOrders.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);
            if (po == null || po.Status == "Received") return false;

            po.Status = "Received";

            foreach (var item in po.Items)
            {
                await _stockService.UpdateStockAsync(
                    item.ProductId,
                    po.BranchId,
                    item.Quantity,
                    TransactionType.Purchase,
                    po.PONumber,
                    $"Received from Vendor via PO {po.PONumber}");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(p => p.Vendor)
                .Include(p => p.Branch)
                .OrderByDescending(p => p.PODate)
                .Select(p => new PurchaseOrderDto
                {
                    Id = p.Id,
                    PONumber = p.PONumber,
                    PODate = p.PODate,
                    VendorId = p.VendorId,
                    VendorName = p.Vendor.Name,
                    BranchId = p.BranchId,
                    BranchName = p.Branch.Name,
                    TotalAmount = p.TotalAmount,
                    TotalTax = p.TotalTax,
                    NetAmount = p.NetAmount,
                    Status = p.Status,
                    Remarks = p.Remarks
                }).ToListAsync();
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(int id)
        {
            var p = await _context.PurchaseOrders
                .Include(po => po.Vendor)
                .Include(po => po.Branch)
                .Include(po => po.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (p == null) return null;

            return new PurchaseOrderDto
            {
                Id = p.Id,
                PONumber = p.PONumber,
                PODate = p.PODate,
                VendorId = p.VendorId,
                VendorName = p.Vendor.Name,
                BranchId = p.BranchId,
                BranchName = p.Branch.Name,
                TotalAmount = p.TotalAmount,
                TotalTax = p.TotalTax,
                NetAmount = p.NetAmount,
                Status = p.Status,
                Remarks = p.Remarks,
                Items = p.Items.Select(i => new PurchaseItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    SKU = i.Product.SKU,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TaxPercentage = i.TaxPercentage,
                    TaxAmount = i.TaxAmount,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }
    }
}

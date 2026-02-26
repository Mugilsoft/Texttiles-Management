using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStockAsync(int productId, int branchId, double quantity, TransactionType type, string referenceNumber = null, string remarks = null)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.BranchId == branchId);

            if (stock == null)
            {
                stock = new Stock
                {
                    ProductId = productId,
                    BranchId = branchId,
                    Quantity = 0,
                    MinStockLevel = 0
                };
                _context.Stocks.Add(stock);
            }

            // Update quantity based on transaction type
            if (type == TransactionType.Purchase || type == TransactionType.StockIn || type == TransactionType.TransferIn)
            {
                stock.Quantity += quantity;
            }
            else if (type == TransactionType.Sale || type == TransactionType.StockOut || type == TransactionType.TransferOut)
            {
                stock.Quantity -= quantity;
            }
            else if (type == TransactionType.Adjustment)
            {
                stock.Quantity += quantity; // Quantity can be negative for reductions
            }

            // Add transaction record
            var transaction = new StockTransaction
            {
                ProductId = productId,
                BranchId = branchId,
                Type = type,
                Quantity = quantity,
                ReferenceNumber = referenceNumber,
                Remarks = remarks
            };
            _context.StockTransactions.Add(transaction);

            await _context.SaveChangesAsync();
        }

        public async Task<double> GetStockLevelAsync(int productId, int branchId)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.BranchId == branchId);
            
            return stock?.Quantity ?? 0;
        }
    }
}

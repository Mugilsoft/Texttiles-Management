using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockService _stockService;

        public InventoryController(ApplicationDbContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        [HttpGet("stock")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetStockLevels(int? branchId)
        {
            var query = _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Branch)
                .AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(s => s.BranchId == branchId.Value);
            }

            var stock = await query.ToListAsync();

            return Ok(stock.Select(s => new
            {
                s.Id,
                s.ProductId,
                ProductName = s.Product?.Name,
                SKU = s.Product?.SKU,
                s.BranchId,
                BranchName = s.Branch?.Name,
                s.Quantity,
                s.MinStockLevel
            }));
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetTransactions(int? branchId, int? productId)
        {
            var query = _context.StockTransactions
                .Include(t => t.Product)
                .Include(t => t.Branch)
                .AsQueryable();

            if (branchId.HasValue)
                query = query.Where(t => t.BranchId == branchId.Value);

            if (productId.HasValue)
                query = query.Where(t => t.ProductId == productId.Value);

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Take(100)
                .ToListAsync();

            return Ok(transactions.Select(t => new
            {
                t.Id,
                t.ProductId,
                ProductName = t.Product?.Name,
                t.BranchId,
                BranchName = t.Branch?.Name,
                t.Type,
                TypeName = t.Type.ToString(),
                t.Quantity,
                t.ReferenceNumber,
                t.Remarks,
                t.CreatedAt
            }));
        }

        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock(StockAdjustmentDto dto)
        {
            await _stockService.UpdateStockAsync(
                dto.ProductId,
                dto.BranchId,
                dto.Quantity,
                TransactionType.Adjustment,
                dto.ReferenceNumber,
                dto.Remarks);

            return Ok(new { Message = "Stock adjusted successfully" });
        }
    }

    public class StockAdjustmentDto
    {
        public int ProductId { get; set; }
        public int BranchId { get; set; }
        public double Quantity { get; set; } // Positive for add, negative for subtract
        public string ReferenceNumber { get; set; }
        public string Remarks { get; set; }
    }
}

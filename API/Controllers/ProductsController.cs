using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Catalog;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;
using TextileBilling.Infrastructure.Data;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly ApplicationDbContext _context; // Access for Include

        public ProductsController(IGenericRepository<Product> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return Ok(products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Barcode = p.Barcode,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                Size = p.Size,
                Color = p.Color,
                Fabric = p.Fabric,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice,
                MRP = p.MRP,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var p = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();

            return Ok(new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Barcode = p.Barcode,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                Size = p.Size,
                Color = p.Color,
                Fabric = p.Fabric,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice,
                MRP = p.MRP,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto dto)
        {
            var p = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Barcode = dto.Barcode,
                CategoryId = dto.CategoryId,
                Size = dto.Size,
                Color = dto.Color,
                Fabric = dto.Fabric,
                PurchasePrice = dto.PurchasePrice,
                SalePrice = dto.SalePrice,
                MRP = dto.MRP,
                TaxPercentage = dto.TaxPercentage,
                IsActive = dto.IsActive
            };

            await _repository.AddAsync(p);
            await _repository.SaveChangesAsync();

            dto.Id = p.Id;
            return CreatedAtAction(nameof(GetProduct), new { id = p.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return NotFound();

            p.Name = dto.Name;
            p.SKU = dto.SKU;
            p.Barcode = dto.Barcode;
            p.CategoryId = dto.CategoryId;
            p.Size = dto.Size;
            p.Color = dto.Color;
            p.Fabric = dto.Fabric;
            p.PurchasePrice = dto.PurchasePrice;
            p.SalePrice = dto.SalePrice;
            p.MRP = dto.MRP;
            p.TaxPercentage = dto.TaxPercentage;
            p.IsActive = dto.IsActive;

            _repository.Update(p);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return NotFound();

            _repository.Delete(p);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Sales;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesInvoiceDto>>> GetSales()
        {
            var sales = await _salesService.GetAllSalesAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesInvoiceDto>> GetSale(int id)
        {
            var sale = await _salesService.GetSalesByIdAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SalesInvoiceDto>> CreateSale(SalesInvoiceDto dto)
        {
            var result = await _salesService.CreateSalesAsync(dto);
            return CreatedAtAction(nameof(GetSale), new { id = result.Id }, result);
        }
    }
}

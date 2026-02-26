using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Purchases;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetPurchases()
        {
            var pos = await _purchaseService.GetAllPurchaseOrdersAsync();
            return Ok(pos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetPurchase(int id)
        {
            var po = await _purchaseService.GetPurchaseOrderByIdAsync(id);
            if (po == null) return NotFound();
            return Ok(po);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> CreatePurchase(PurchaseOrderDto dto)
        {
            var result = await _purchaseService.CreatePurchaseOrderAsync(dto);
            return CreatedAtAction(nameof(GetPurchase), new { id = result.Id }, result);
        }

        [HttpPost("{id}/receive")]
        public async Task<IActionResult> ReceivePurchase(int id)
        {
            var success = await _purchaseService.ReceivePurchaseOrderAsync(id);
            if (!success) return BadRequest("Could not receive purchase order. It may be already received or not found.");
            return Ok(new { Message = "Purchase received and stock updated" });
        }
    }
}

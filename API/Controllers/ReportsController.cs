using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Reports;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("daybook")]
        public async Task<ActionResult<DayBookDto>> GetDayBook(DateTime? date, int? branchId)
        {
            var targetDate = date ?? DateTime.Today;
            var result = await _reportService.GetDayBookAsync(targetDate, branchId);
            return Ok(result);
        }

        [HttpGet("sales-report")]
        public async Task<ActionResult<IEnumerable<SalesReportDto>>> GetSalesReport(DateTime from, DateTime to, int? branchId)
        {
            var result = await _reportService.GetSalesReportAsync(from, to, branchId);
            return Ok(result);
        }

        [HttpGet("purchase-report")]
        public async Task<ActionResult<IEnumerable<PurchaseReportDto>>> GetPurchaseReport(DateTime from, DateTime to, int? branchId)
        {
            var result = await _reportService.GetPurchaseReportAsync(from, to, branchId);
            return Ok(result);
        }
    }
}

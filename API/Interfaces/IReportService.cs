using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Reports;

namespace TextileBilling.Core.Interfaces
{
    public interface IReportService
    {
        Task<DayBookDto> GetDayBookAsync(DateTime date, int? branchId);
        Task<IEnumerable<SalesReportDto>> GetSalesReportAsync(DateTime from, DateTime to, int? branchId);
        Task<IEnumerable<PurchaseReportDto>> GetPurchaseReportAsync(DateTime from, DateTime to, int? branchId);
    }
}

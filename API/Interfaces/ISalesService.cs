using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Sales;

namespace TextileBilling.Core.Interfaces
{
    public interface ISalesService
    {
        Task<IEnumerable<SalesInvoiceDto>> GetAllSalesAsync();
        Task<SalesInvoiceDto> GetSalesByIdAsync(int id);
        Task<SalesInvoiceDto> CreateSalesAsync(SalesInvoiceDto dto);
    }
}

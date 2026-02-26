using System.Collections.Generic;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Purchases;

namespace TextileBilling.Core.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(PurchaseOrderDto dto);
        Task<bool> ReceivePurchaseOrderAsync(int id);
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(int id);
    }
}

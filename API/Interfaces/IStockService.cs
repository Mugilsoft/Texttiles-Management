using System.Threading.Tasks;

namespace TextileBilling.Core.Interfaces
{
    public interface IStockService
    {
        Task UpdateStockAsync(int productId, int branchId, double quantity, TextileBilling.Core.Entities.TransactionType type, string referenceNumber = null, string remarks = null);
        Task<double> GetStockLevelAsync(int productId, int branchId);
    }
}

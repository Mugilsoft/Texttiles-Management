using System;
using TextileBilling.Core.Common;

namespace TextileBilling.Core.Entities
{
    public enum TransactionType
    {
        Purchase,
        Sale,
        StockIn,
        StockOut,
        TransferIn,
        TransferOut,
        Adjustment
    }

    public class StockTransaction : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public TransactionType Type { get; set; }
        public double Quantity { get; set; }
        public string ReferenceNumber { get; set; } // PO Number, Invoice Number, etc.
        public string Remarks { get; set; }
    }
}

namespace TextileBilling.Core.DTOs.Catalog
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Fabric { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal MRP { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}

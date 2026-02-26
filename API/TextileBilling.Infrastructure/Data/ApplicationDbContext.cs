using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TextileBilling.Core.Common;
using TextileBilling.Core.Entities;

namespace TextileBilling.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Branch - Counter relationship
            builder.Entity<Counter>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Counters)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Branch relationship
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Category relationship
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Stock configuration
            builder.Entity<Stock>()
                .HasIndex(s => new { s.ProductId, s.BranchId })
                .IsUnique();

            // Sales configuration
            builder.Entity<SalesInvoice>()
                .HasMany(s => s.Items)
                .WithOne(i => i.SalesInvoice)
                .HasForeignKey(i => i.SalesInvoiceId);

            builder.Entity<SalesInvoice>()
                .Property(s => s.TotalAmount).HasPrecision(18, 2);
            builder.Entity<SalesInvoice>()
                .Property(s => s.TotalTax).HasPrecision(18, 2);
            builder.Entity<SalesInvoice>()
                .Property(s => s.Discount).HasPrecision(18, 2);
            builder.Entity<SalesInvoice>()
                .Property(s => s.NetAmount).HasPrecision(18, 2);

            builder.Entity<SalesItem>()
                .Property(i => i.UnitPrice).HasPrecision(18, 2);
            builder.Entity<SalesItem>()
                .Property(i => i.TaxAmount).HasPrecision(18, 2);
            builder.Entity<SalesItem>()
                .Property(i => i.Discount).HasPrecision(18, 2);
            builder.Entity<SalesItem>()
                .Property(i => i.SubTotal).HasPrecision(18, 2);

            // Purchase configuration
            builder.Entity<PurchaseOrder>()
                .HasMany(p => p.Items)
                .WithOne(i => i.PurchaseOrder)
                .HasForeignKey(i => i.PurchaseOrderId);

            builder.Entity<PurchaseOrder>()
                .Property(p => p.TotalAmount).HasPrecision(18, 2);
            builder.Entity<PurchaseOrder>()
                .Property(p => p.TotalTax).HasPrecision(18, 2);
            builder.Entity<PurchaseOrder>()
                .Property(p => p.NetAmount).HasPrecision(18, 2);

            builder.Entity<PurchaseItem>()
                .Property(i => i.UnitPrice).HasPrecision(18, 2);
            builder.Entity<PurchaseItem>()
                .Property(i => i.TaxAmount).HasPrecision(18, 2);
            builder.Entity<PurchaseItem>()
                .Property(i => i.SubTotal).HasPrecision(18, 2);

            // Decimal precision for products
            builder.Entity<Product>()
                .Property(p => p.PurchasePrice).HasPrecision(18, 2);
            builder.Entity<Product>()
                .Property(p => p.SalePrice).HasPrecision(18, 2);
            builder.Entity<Product>()
                .Property(p => p.MRP).HasPrecision(18, 2);
            builder.Entity<Product>()
                .Property(p => p.TaxPercentage).HasPrecision(18, 2);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "System"; // Ideally get current user from IHttpContextAccessor
                        entry.Entity.LastModifiedBy = "System";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "System";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

using CaffePOS.Model;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets - Tất cả models
        public DbSet<Category> Category { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }


        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();  

            // Cấu hình triggers
            modelBuilder.Entity<Items>()
                .ToTable(tb => tb.HasTrigger("TR_Items_Update"));

            modelBuilder.Entity<Users>()
                .ToTable(tb => tb.HasTrigger("Users_AuditLog"));

            // Cấu hình relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)//1 Order thuộc về 1 User
                .WithMany(u => u.Orders)// 1 User có nhiều Order
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);// Không cho xóa User nếu còn Orders

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany()
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payments>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Items>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình decimal precision
            modelBuilder.Entity<Items>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FinalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PriceAtSale)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payments>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Gọi hàm base ở cuối cùng
            base.OnModelCreating(modelBuilder);
        }
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                // Kiểm tra xem entity có các property timestamp không
                var createdAtProperty = entry.Metadata.FindProperty("CreatedAt");
                var updatedAtProperty = entry.Metadata.FindProperty("UpdatedAt");

                // Xử lý cho các entity có CreatedAt và UpdatedAt
                if (entry.State == EntityState.Added)
                {
                    if (createdAtProperty != null)
                    {
                        entry.Property("CreatedAt").CurrentValue = now;
                    }

                    if (updatedAtProperty != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (updatedAtProperty != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                    if (createdAtProperty != null)
                    {
                        entry.Property("CreatedAt").IsModified = false;
                    }
                }
            }
        }
    }
}
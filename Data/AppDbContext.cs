using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Models;
using UnityAssetStore.Models.Identity;

namespace UnityAssetStore.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Настройка связей один-ко-многим
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId);

            // 🔹 Настройка связей один-ко-многим (Order - OrderItem)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Asset)
                .WithMany()
                .HasForeignKey(oi => oi.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Asset)
                .WithMany()
                .HasForeignKey(oi => oi.AssetId); 
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Asset)
                .WithMany()
                .HasForeignKey(c => c.AssetId);
        }
    }
}
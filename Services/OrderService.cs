using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnityAssetStore.Data;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Asset)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Asset)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> CreateOrderFromCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Asset)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("Корзина пуста");

            decimal totalAmount = cartItems.Sum(c => c.Quantity * c.Asset.Price);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Processing",
                TotalAmount = totalAmount,
                Items = cartItems.Select(item => new OrderItem
                {
                    AssetId = item.AssetId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Asset.Price
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        public async Task<int> CreateSingleItemOrderAsync(string userId, int assetId)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null)
                throw new InvalidOperationException("Товар не найден");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Processing",
                TotalAmount = asset.Price,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        AssetId = assetId,
                        Quantity = 1,
                        UnitPrice = asset.Price
                    }
                }
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order.Id;
        }
    }
}
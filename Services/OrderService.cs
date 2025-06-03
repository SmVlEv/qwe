using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Models.Identity;
using UnityAssetStore.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
        if (string.IsNullOrEmpty(userId))
            throw new InvalidOperationException("Пользователь не найден");

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Asset)
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
}
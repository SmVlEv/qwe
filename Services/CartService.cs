using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Services;
using System.Linq;
using System.Threading.Tasks;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private const string SessionKey = "ShoppingCart";

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    // --- Работа с Session (гости) ---

    public ShoppingCart GetCart(ISession session)
    {
        var cartJson = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(cartJson))
            return new ShoppingCart();

        var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);

        // Загружаем Asset для отображения
        foreach (var item in cart.Items)
        {
            item.Asset = _context.Assets.Find(item.AssetId);
        }

        return cart;
    }

    public ShoppingCart AddToCart(ISession session, int assetId)
    {
        var cart = GetCart(session);

        var asset = _context.Assets.Find(assetId);
        if (asset == null)
            throw new ArgumentException("Товар не найден", nameof(assetId));

        var item = cart.Items.FirstOrDefault(i => i.AssetId == assetId);
        if (item == null)
        {
            cart.Items.Add(new CartItem { AssetId = assetId, Quantity = 1 });
        }
        else
        {
            item.Quantity++;
        }

        SaveCart(session, cart);
        return cart;
    }

    public ShoppingCart RemoveFromCart(ISession session, int assetId)
    {
        var cart = GetCart(session);
        var item = cart.Items.FirstOrDefault(i => i.AssetId == assetId);

        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCart(session, cart);
        }

        return cart;
    }

    public void ClearCart(ISession session)
    {
        session.Remove(SessionKey);
    }

    private void SaveCart(ISession session, ShoppingCart cart)
    {
        var cartJson = JsonConvert.SerializeObject(cart);
        session.SetString(SessionKey, cartJson);
    }

    // --- Работа с UserId (авторизованные пользователи) ---

    public async Task AddToCartAsync(string userId, int assetId)
    {
        var existingItem = await _context.CartItems
            .Where(c => c.UserId == userId && c.AssetId == assetId)
            .Include(c => c.Asset)
            .FirstOrDefaultAsync();

        if (existingItem == null)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null)
                throw new ArgumentException("Товар не найден", nameof(assetId));

            await _context.CartItems.AddAsync(new CartItem
            {
                UserId = userId,
                AssetId = assetId,
                Quantity = 1
            });
        }
        else
        {
            existingItem.Quantity += 1;
            _context.CartItems.Update(existingItem);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(string userId, int assetId)
    {
        var item = await _context.CartItems
            .Include(c => c.Asset)
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync(c => c.AssetId == assetId);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public IEnumerable<CartItem> GetCartItems(string userId)
    {
        return _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Asset)
            .ToList();
    }
}
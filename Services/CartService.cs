using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private const string SessionKey = "ShoppingCart";

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    // --- Работа с Session (для гостей) ---

    public ShoppingCart GetCart(ISession session)
    {
        var cartJson = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(cartJson))
            return new ShoppingCart();

        var cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
        if (cart == null)
            return new ShoppingCart();

        foreach (var item in cart.Items)
        {
            item.Asset = _context.Assets.Find(item.AssetId);
        }

        return cart;
    }

    public void AddToCart(ISession session, int assetId)
    {
        var cart = GetCart(session);
        var existingItem = cart.Items.FirstOrDefault(i => i.AssetId == assetId);

        var asset = _context.Assets.Find(assetId);
        if (asset == null)
            throw new ArgumentException("Товар не найден", nameof(assetId));

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItem { AssetId = assetId, Quantity = 1 });
        }

        SaveCart(session, cart);
    }

    public void RemoveFromCart(ISession session, int assetId)
    {
        var cart = GetCart(session);
        var item = cart.Items.FirstOrDefault(i => i.AssetId == assetId);

        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCart(session, cart);
        }
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

    // --- Работа с пользователем (UserId) ---

    public async Task AddToCartAsync(string userId, int assetId)
    {
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AssetId == assetId);

        if (existingItem != null)
        {
            existingItem.Quantity += 1;
            _context.CartItems.Update(existingItem);
        }
        else
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

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(string userId, int assetId)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AssetId == assetId);

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

    // --- Метод для переноса из Session в UserId ---

    public async Task TransferCartFromSessionToUserAsync(ISession session, string userId)
    {
        var cart = GetCart(session);
        foreach (var item in cart.Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                await AddToCartAsync(userId, item.AssetId);
            }
        }

        ClearCart(session); // Очистка корзины гостя
    }
}
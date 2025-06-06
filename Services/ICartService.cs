using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public interface ICartService
    {
        // Работа с Session (для гостей)
        ShoppingCart GetCart(ISession session);
        void AddToCart(ISession session, int assetId);
        void RemoveFromCart(ISession session, int assetId);
        void ClearCart(ISession session);

        // Работа с пользователем (UserId)
        Task AddToCartAsync(string userId, int assetId);
        Task RemoveFromCartAsync(string userId, int assetId);
        IEnumerable<CartItem> GetCartItems(string userId);

        // Перенос из Session в User.Cart
        Task TransferCartFromSessionToUserAsync(ISession session, string userId);
    }
}
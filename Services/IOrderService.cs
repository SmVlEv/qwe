using System.Threading.Tasks;
using System.Collections.Generic;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<int> CreateOrderFromCartAsync(string userId);
    }
}
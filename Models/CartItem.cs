using System.ComponentModel.DataAnnotations;

namespace UnityAssetStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? UserId { get; set; }
        public Guid? SessionId { get; set; }
        public Asset? Asset { get; set; }
    }
}
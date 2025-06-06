using System.Collections.Generic;

namespace UnityAssetStore.Models
{
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();

        public decimal TotalPrice()
        {
            return Items.Sum(i => i.Asset?.Price * i.Quantity ?? 0);
        }
    }
}
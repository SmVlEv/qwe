namespace UnityAssetStore.Models
{
    public class ShoppingCart
    {
        public string UserId { get; set; } // Можно использовать Session ID или IdentityUser.Id
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalPrice()
        {
            return Items.Sum(i => i.Asset.Price * i.Quantity);
        }
    }
}

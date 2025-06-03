namespace UnityAssetStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

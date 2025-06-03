namespace UnityAssetStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // Ссылка на пользователя (например, IdentityUser.Id)

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Processing"; // Processing / Completed / Cancelled

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}

namespace UnityAssetStore.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User"; // User / Admin
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

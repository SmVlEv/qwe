using UnityAssetStore.Models;

public class CartItem
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public int Quantity { get; set; }

    // Ссылка на товар
    public Asset Asset { get; set; }

    // Пользователь (только если он авторизован)
    public string? UserId { get; set; }

    // Анонимная сессия (на случай, если пользователь не вошёл)
    public Guid? SessionId { get; set; }
}
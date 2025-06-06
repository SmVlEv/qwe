using System.ComponentModel.DataAnnotations;

namespace UnityAssetStore.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Описание товара обязательно")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0, 9999.99, ErrorMessage = "Цена должна быть положительной и не более 9999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "URL изображения обязателен")]
        [Display(Name = "URL превью изображения")]
        public string PreviewImageUrl { get; set; } = "/img/default.jpg";

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
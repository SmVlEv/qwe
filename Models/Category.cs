using System.ComponentModel.DataAnnotations;

namespace UnityAssetStore.Models
{
    public class Category
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        [Display(Name = "Название категории")]
        public string Name { get; set; } = null!;

        // Связь с товарами (один ко многим)
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace UnityAssetStore.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [Display(Name = "Имя пользователя")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        [Display(Name = "Вернуться на предыдущую страницу")]
        public string? ReturnUrl { get; set; }
    }
}
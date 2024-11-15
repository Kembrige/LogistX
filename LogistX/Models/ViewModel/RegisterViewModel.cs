using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LogistX.Models.ViewModel
{
    /// <summary>
    /// Модель для регистрации пользователя.
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Электронная почта обязательна")]
        [EmailAddress(ErrorMessage = "Некорректный формат электронной почты")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Роль обязательна")]
        [Display(Name = "Роль")]
        public string Role { get; set; }
    }
}
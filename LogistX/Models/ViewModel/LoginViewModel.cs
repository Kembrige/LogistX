using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LogistX.Models.ViewModel
{
    /// <summary>
    /// Модель для входа пользователя.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}

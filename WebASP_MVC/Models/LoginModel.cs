#if role || cookies
using System.ComponentModel.DataAnnotations;
#endif

namespace WebASP_MVC.Models
{
    public class LoginModel
    {
#if role || cookies
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

#else
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
#endif
    }
}
